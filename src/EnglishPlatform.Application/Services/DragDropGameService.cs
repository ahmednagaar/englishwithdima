using EnglishPlatform.Application.DTOs.Games;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.UnitOfWork;
using EnglishPlatform.Shared;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Application.Services;

public class DragDropGameService : IDragDropGameService
{
    private readonly IUnitOfWork _uow;
    public DragDropGameService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedList<DragDropGameDto>>> GetGamesAsync(GameFilterDto filter)
    {
        var query = _uow.DragDropQuestions.Query().Include(d => d.Zones).Include(d => d.Items).Where(d => d.IsActive);
        if (filter.GradeId.HasValue) query = query.Where(d => d.GradeId == filter.GradeId.Value);
        if (filter.SkillCategory.HasValue) query = query.Where(d => d.SkillCategory == filter.SkillCategory.Value);

        var pagedList = await query.OrderBy(d => d.DisplayOrder)
            .Select(d => MapToDto(d))
            .ToPagedListAsync(filter.PageNumber, filter.PageSize);
        return Result<PagedList<DragDropGameDto>>.Ok(pagedList);
    }

    public async Task<Result<DragDropGameDto>> GetByIdAsync(int id)
    {
        var game = await _uow.DragDropQuestions.Query().Include(d => d.Zones).Include(d => d.Items).FirstOrDefaultAsync(d => d.Id == id);
        if (game == null) return Result<DragDropGameDto>.Fail("Game not found");
        return Result<DragDropGameDto>.Ok(MapToDto(game));
    }

    public async Task<Result<DragDropGameDto>> CreateAsync(CreateDragDropGameDto dto, string userId)
    {
        var game = new DragDropQuestion
        {
            GameTitle = dto.GameTitle, Instructions = dto.Instructions, GradeId = dto.GradeId,
            SkillCategory = dto.SkillCategory, ContentTopic = dto.ContentTopic,
            DifficultyLevel = dto.DifficultyLevel, TimeLimit = dto.TimeLimit,
            PointsPerCorrectItem = dto.PointsPerCorrectItem, ShowImmediateFeedback = dto.ShowImmediateFeedback,
            ThumbnailUrl = dto.ThumbnailUrl, NumberOfZones = dto.Zones.Count,
            CreatedBy = userId, IsActive = true
        };
        await _uow.DragDropQuestions.AddAsync(game);
        await _uow.SaveChangesAsync();

        var zoneMap = new Dictionary<int, int>(); // index -> real ID
        for (int i = 0; i < dto.Zones.Count; i++)
        {
            var z = dto.Zones[i];
            var zone = new DragDropZone { DragDropQuestionId = game.Id, ZoneLabel = z.ZoneLabel, ZoneColor = z.ZoneColor, ImageUrl = z.ImageUrl, ZoneOrder = z.ZoneOrder };
            await _uow.DragDropZones.AddAsync(zone);
            await _uow.SaveChangesAsync();
            zoneMap[i] = zone.Id;
        }

        foreach (var itemDto in dto.Items)
        {
            var item = new DragDropItem
            {
                DragDropQuestionId = game.Id, ItemText = itemDto.ItemText,
                ItemImageUrl = itemDto.ItemImageUrl, ItemAudioUrl = itemDto.ItemAudioUrl,
                CorrectZoneId = zoneMap.GetValueOrDefault(itemDto.CorrectZoneIndex, zoneMap.Values.First()),
                Explanation = itemDto.Explanation, ItemOrder = itemDto.ItemOrder
            };
            await _uow.DragDropItems.AddAsync(item);
        }
        await _uow.SaveChangesAsync();
        return await GetByIdAsync(game.Id);
    }

    public async Task<Result<DragDropGameDto>> UpdateAsync(int id, CreateDragDropGameDto dto, string userId)
    {
        var game = await _uow.DragDropQuestions.Query().Include(d => d.Zones).Include(d => d.Items).FirstOrDefaultAsync(d => d.Id == id);
        if (game == null) return Result<DragDropGameDto>.Fail("Game not found");

        game.GameTitle = dto.GameTitle; game.Instructions = dto.Instructions;
        game.GradeId = dto.GradeId; game.UpdatedBy = userId;
        _uow.DragDropQuestions.Update(game);
        await _uow.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var game = await _uow.DragDropQuestions.GetByIdAsync(id);
        if (game == null) return Result.Fail("Game not found");
        _uow.DragDropQuestions.Delete(game);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<DragDropGameDto>> StartSessionAsync(int gameId, string? userId)
    {
        var game = await _uow.DragDropQuestions.Query().Include(d => d.Zones).Include(d => d.Items).FirstOrDefaultAsync(d => d.Id == gameId);
        if (game == null) return Result<DragDropGameDto>.Fail("Game not found");

        var session = new DragDropGameSession
        {
            UserId = userId, DragDropQuestionId = gameId, TotalItems = game.Items.Count, StartTime = DateTime.UtcNow
        };
        await _uow.DragDropGameSessions.AddAsync(session);
        await _uow.SaveChangesAsync();

        var dto = MapToDto(game);
        dto.Items = dto.Items.OrderBy(_ => Guid.NewGuid()).ToList(); // Shuffle items
        return Result<DragDropGameDto>.Ok(dto);
    }

    public async Task<Result<GameSessionResultDto>> SubmitSessionAsync(DragDropSubmitDto dto, string? userId)
    {
        var session = await _uow.DragDropGameSessions.Query()
            .Include(s => s.DragDropQuestion).ThenInclude(d => d.Items)
            .FirstOrDefaultAsync(s => s.Id == dto.SessionId);
        if (session == null) return Result<GameSessionResultDto>.Fail("Session not found");

        int correct = 0, wrong = 0;
        foreach (var move in dto.Moves)
        {
            var item = session.DragDropQuestion.Items.FirstOrDefault(i => i.Id == move.ItemId);
            bool isCorrect = item != null && item.CorrectZoneId == move.PlacedInZoneId;
            if (isCorrect) correct++; else wrong++;

            await _uow.DragDropAttempts.AddAsync(new DragDropAttempt
            {
                SessionId = dto.SessionId, ItemId = move.ItemId,
                PlacedInZoneId = move.PlacedInZoneId, IsCorrect = isCorrect, TimeSpentMs = move.TimeSpentMs
            });
        }

        int score = correct * session.DragDropQuestion.PointsPerCorrectItem;
        session.CorrectPlacements = correct; session.WrongPlacements = wrong;
        session.TotalScore = score; session.TimeSpentSeconds = dto.TimeSpentSeconds;
        session.EndTime = DateTime.UtcNow; session.IsCompleted = true;
        _uow.DragDropGameSessions.Update(session);
        await _uow.SaveChangesAsync();

        return Result<GameSessionResultDto>.Ok(new GameSessionResultDto
        {
            SessionId = session.Id, GameId = session.DragDropQuestionId, GameType = "DragDrop",
            TotalScore = score, CorrectCount = correct, WrongCount = wrong,
            TimeSpentSeconds = dto.TimeSpentSeconds, IsCompleted = true, PointsEarned = score
        });
    }

    private static DragDropGameDto MapToDto(DragDropQuestion d) => new()
    {
        Id = d.Id, GameTitle = d.GameTitle, Instructions = d.Instructions, GradeId = d.GradeId,
        SkillCategory = d.SkillCategory, DifficultyLevel = d.DifficultyLevel, TimeLimit = d.TimeLimit,
        PointsPerCorrectItem = d.PointsPerCorrectItem, ThumbnailUrl = d.ThumbnailUrl,
        Zones = d.Zones.OrderBy(z => z.ZoneOrder).Select(z => new DragDropZoneDto { Id = z.Id, ZoneLabel = z.ZoneLabel, ZoneColor = z.ZoneColor, ImageUrl = z.ImageUrl }).ToList(),
        Items = d.Items.OrderBy(i => i.ItemOrder).Select(i => new DragDropItemDto { Id = i.Id, ItemText = i.ItemText, ItemImageUrl = i.ItemImageUrl, ItemAudioUrl = i.ItemAudioUrl }).ToList()
    };
}
