using EnglishPlatform.Application.DTOs.Games;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.UnitOfWork;
using EnglishPlatform.Shared;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Application.Services;

public class FlipCardGameService : IFlipCardGameService
{
    private readonly IUnitOfWork _uow;
    public FlipCardGameService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedList<FlipCardGameDto>>> GetGamesAsync(GameFilterDto filter)
    {
        var query = _uow.FlipCardQuestions.Query().Include(f => f.Pairs).Where(f => f.IsActive);
        if (filter.GradeId.HasValue) query = query.Where(f => f.GradeId == filter.GradeId.Value);
        if (filter.SkillCategory.HasValue) query = query.Where(f => f.SkillCategory == filter.SkillCategory.Value);

        var pagedList = await query.OrderBy(f => f.DisplayOrder)
            .Select(f => MapToDto(f))
            .ToPagedListAsync(filter.PageNumber, filter.PageSize);
        return Result<PagedList<FlipCardGameDto>>.Ok(pagedList);
    }

    public async Task<Result<FlipCardGameDto>> GetByIdAsync(int id)
    {
        var game = await _uow.FlipCardQuestions.Query().Include(f => f.Pairs).FirstOrDefaultAsync(f => f.Id == id);
        if (game == null) return Result<FlipCardGameDto>.Fail("Game not found");
        return Result<FlipCardGameDto>.Ok(MapToDto(game));
    }

    public async Task<Result<FlipCardGameDto>> CreateAsync(CreateFlipCardGameDto dto, string userId)
    {
        var game = new FlipCardQuestion
        {
            GameTitle = dto.GameTitle, Instructions = dto.Instructions, GradeId = dto.GradeId,
            SkillCategory = dto.SkillCategory, ContentTopic = dto.ContentTopic,
            GameMode = dto.GameMode, NumberOfPairs = dto.NumberOfPairs,
            DifficultyLevel = dto.DifficultyLevel, TimerMode = dto.TimerMode,
            TimeLimitSeconds = dto.TimeLimitSeconds, PointsPerMatch = dto.PointsPerMatch,
            ShowHints = dto.ShowHints, MaxHints = dto.MaxHints,
            CardBackDesign = dto.CardBackDesign, ThumbnailUrl = dto.ThumbnailUrl,
            CreatedBy = userId, IsActive = true
        };
        await _uow.FlipCardQuestions.AddAsync(game);
        await _uow.SaveChangesAsync();

        foreach (var p in dto.Pairs)
        {
            await _uow.FlipCardPairs.AddAsync(new FlipCardPair
            {
                FlipCardQuestionId = game.Id,
                Card1Type = p.Card1Type, Card1Text = p.Card1Text,
                Card1ImageUrl = p.Card1ImageUrl, Card1AudioUrl = p.Card1AudioUrl,
                Card2Type = p.Card2Type, Card2Text = p.Card2Text,
                Card2ImageUrl = p.Card2ImageUrl, Card2AudioUrl = p.Card2AudioUrl,
                Explanation = p.Explanation, PairOrder = p.PairOrder
            });
        }
        await _uow.SaveChangesAsync();
        return await GetByIdAsync(game.Id);
    }

    public async Task<Result<FlipCardGameDto>> UpdateAsync(int id, CreateFlipCardGameDto dto, string userId)
    {
        var game = await _uow.FlipCardQuestions.Query().Include(f => f.Pairs).FirstOrDefaultAsync(f => f.Id == id);
        if (game == null) return Result<FlipCardGameDto>.Fail("Game not found");

        game.GameTitle = dto.GameTitle; game.Instructions = dto.Instructions;
        game.GradeId = dto.GradeId; game.SkillCategory = dto.SkillCategory;
        game.GameMode = dto.GameMode; game.DifficultyLevel = dto.DifficultyLevel;
        game.TimerMode = dto.TimerMode; game.TimeLimitSeconds = dto.TimeLimitSeconds;
        game.PointsPerMatch = dto.PointsPerMatch; game.UpdatedBy = userId;

        foreach (var old in game.Pairs.ToList()) _uow.FlipCardPairs.Delete(old);
        foreach (var p in dto.Pairs)
        {
            await _uow.FlipCardPairs.AddAsync(new FlipCardPair
            {
                FlipCardQuestionId = id,
                Card1Type = p.Card1Type, Card1Text = p.Card1Text,
                Card1ImageUrl = p.Card1ImageUrl, Card1AudioUrl = p.Card1AudioUrl,
                Card2Type = p.Card2Type, Card2Text = p.Card2Text,
                Card2ImageUrl = p.Card2ImageUrl, Card2AudioUrl = p.Card2AudioUrl,
                Explanation = p.Explanation, PairOrder = p.PairOrder
            });
        }
        game.NumberOfPairs = dto.Pairs.Count;
        _uow.FlipCardQuestions.Update(game);
        await _uow.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var game = await _uow.FlipCardQuestions.GetByIdAsync(id);
        if (game == null) return Result.Fail("Game not found");
        _uow.FlipCardQuestions.Delete(game);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<FlipCardGameDto>> StartSessionAsync(int gameId, string? userId)
    {
        var game = await _uow.FlipCardQuestions.Query().Include(f => f.Pairs).FirstOrDefaultAsync(f => f.Id == gameId);
        if (game == null) return Result<FlipCardGameDto>.Fail("Game not found");

        var session = new FlipCardGameSession
        {
            UserId = userId, FlipCardQuestionId = gameId, TotalPairs = game.NumberOfPairs, StartTime = DateTime.UtcNow
        };
        await _uow.FlipCardGameSessions.AddAsync(session);
        await _uow.SaveChangesAsync();

        return Result<FlipCardGameDto>.Ok(MapToDto(game));
    }

    public async Task<Result<GameSessionResultDto>> SubmitSessionAsync(FlipCardSubmitDto dto, string? userId)
    {
        var session = await _uow.FlipCardGameSessions.Query()
            .Include(s => s.FlipCardQuestion).FirstOrDefaultAsync(s => s.Id == dto.SessionId);
        if (session == null) return Result<GameSessionResultDto>.Fail("Session not found");

        var game = session.FlipCardQuestion;
        int score = (dto.MatchesFound * game.PointsPerMatch) - (dto.WrongFlips * game.MovePenalty);
        score = Math.Max(0, score);

        session.MatchesFound = dto.MatchesFound; session.TotalFlips = dto.TotalFlips;
        session.WrongFlips = dto.WrongFlips; session.HintsUsed = dto.HintsUsed;
        session.TotalScore = score; session.TimeSpentSeconds = dto.TimeSpentSeconds;
        session.EndTime = DateTime.UtcNow; session.IsCompleted = true;
        _uow.FlipCardGameSessions.Update(session);
        await _uow.SaveChangesAsync();

        return Result<GameSessionResultDto>.Ok(new GameSessionResultDto
        {
            SessionId = session.Id, GameId = game.Id, GameType = "FlipCard",
            TotalScore = score, CorrectCount = dto.MatchesFound, WrongCount = dto.WrongFlips,
            TimeSpentSeconds = dto.TimeSpentSeconds, HintsUsed = dto.HintsUsed,
            IsCompleted = true, PointsEarned = score
        });
    }

    private static FlipCardGameDto MapToDto(FlipCardQuestion f) => new()
    {
        Id = f.Id, GameTitle = f.GameTitle, Instructions = f.Instructions, GradeId = f.GradeId,
        SkillCategory = f.SkillCategory, GameMode = f.GameMode, NumberOfPairs = f.NumberOfPairs,
        DifficultyLevel = f.DifficultyLevel, TimerMode = f.TimerMode,
        TimeLimitSeconds = f.TimeLimitSeconds, PointsPerMatch = f.PointsPerMatch,
        ShowHints = f.ShowHints, ThumbnailUrl = f.ThumbnailUrl, CardBackDesign = f.CardBackDesign,
        Pairs = f.Pairs.OrderBy(p => p.PairOrder).Select(p => new FlipCardPairDto
        {
            Id = p.Id, Card1Text = p.Card1Text, Card1ImageUrl = p.Card1ImageUrl,
            Card1AudioUrl = p.Card1AudioUrl, Card2Text = p.Card2Text,
            Card2ImageUrl = p.Card2ImageUrl, Card2AudioUrl = p.Card2AudioUrl, Explanation = p.Explanation
        }).ToList()
    };
}
