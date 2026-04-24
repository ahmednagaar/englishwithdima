using EnglishPlatform.Application.DTOs.Games;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.UnitOfWork;
using EnglishPlatform.Shared;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Application.Services;

public class MatchingGameService : IMatchingGameService
{
    private readonly IUnitOfWork _uow;
    public MatchingGameService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<PagedList<MatchingGameDto>>> GetGamesAsync(GameFilterDto filter)
    {
        var query = _uow.MatchingGames.Query()
            .Include(m => m.Pairs)
            .Where(m => m.IsActive);

        if (filter.GradeId.HasValue) query = query.Where(m => m.GradeId == filter.GradeId.Value);
        if (filter.SkillCategory.HasValue) query = query.Where(m => m.SkillCategory == filter.SkillCategory.Value);
        if (filter.DifficultyLevel.HasValue) query = query.Where(m => m.DifficultyLevel == filter.DifficultyLevel.Value);

        var pagedList = await query.OrderBy(m => m.DisplayOrder)
            .Select(m => MapToDto(m))
            .ToPagedListAsync(filter.PageNumber, filter.PageSize);

        return Result<PagedList<MatchingGameDto>>.Ok(pagedList);
    }

    public async Task<Result<MatchingGameDto>> GetByIdAsync(int id)
    {
        var game = await _uow.MatchingGames.Query().Include(m => m.Pairs).FirstOrDefaultAsync(m => m.Id == id);
        if (game == null) return Result<MatchingGameDto>.Fail("Game not found");
        return Result<MatchingGameDto>.Ok(MapToDto(game));
    }

    public async Task<Result<MatchingGameDto>> CreateAsync(CreateMatchingGameDto dto, string userId)
    {
        var game = new MatchingGame
        {
            GameTitle = dto.GameTitle, Instructions = dto.Instructions, GradeId = dto.GradeId,
            SkillCategory = dto.SkillCategory, ContentTopic = dto.ContentTopic, MatchingMode = dto.MatchingMode,
            TimerMode = dto.TimerMode, TimeLimitSeconds = dto.TimeLimitSeconds, PointsPerMatch = dto.PointsPerMatch,
            WrongMatchPenalty = dto.WrongMatchPenalty, DifficultyLevel = dto.DifficultyLevel,
            EnableHints = dto.EnableHints, MaxHints = dto.MaxHints, Category = dto.Category,
            ThumbnailUrl = dto.ThumbnailUrl, NumberOfPairs = dto.Pairs.Count,
            CreatedBy = userId, IsActive = true
        };
        await _uow.MatchingGames.AddAsync(game);
        await _uow.SaveChangesAsync();

        foreach (var p in dto.Pairs)
        {
            await _uow.MatchingGamePairs.AddAsync(new MatchingGamePair
            {
                MatchingGameId = game.Id, QuestionText = p.QuestionText, AnswerText = p.AnswerText,
                QuestionImageUrl = p.QuestionImageUrl, AnswerImageUrl = p.AnswerImageUrl,
                QuestionAudioUrl = p.QuestionAudioUrl, AnswerAudioUrl = p.AnswerAudioUrl,
                QuestionType = p.QuestionType, AnswerType = p.AnswerType,
                Explanation = p.Explanation, PairOrder = p.PairOrder
            });
        }
        await _uow.SaveChangesAsync();
        return await GetByIdAsync(game.Id);
    }

    public async Task<Result<MatchingGameDto>> UpdateAsync(int id, CreateMatchingGameDto dto, string userId)
    {
        var game = await _uow.MatchingGames.Query().Include(m => m.Pairs).FirstOrDefaultAsync(m => m.Id == id);
        if (game == null) return Result<MatchingGameDto>.Fail("Game not found");

        game.GameTitle = dto.GameTitle; game.Instructions = dto.Instructions;
        game.GradeId = dto.GradeId; game.SkillCategory = dto.SkillCategory;
        game.MatchingMode = dto.MatchingMode; game.TimerMode = dto.TimerMode;
        game.TimeLimitSeconds = dto.TimeLimitSeconds; game.PointsPerMatch = dto.PointsPerMatch;
        game.DifficultyLevel = dto.DifficultyLevel; game.EnableHints = dto.EnableHints;
        game.MaxHints = dto.MaxHints; game.UpdatedBy = userId;

        foreach (var old in game.Pairs.ToList()) _uow.MatchingGamePairs.Delete(old);
        foreach (var p in dto.Pairs)
        {
            await _uow.MatchingGamePairs.AddAsync(new MatchingGamePair
            {
                MatchingGameId = id, QuestionText = p.QuestionText, AnswerText = p.AnswerText,
                QuestionImageUrl = p.QuestionImageUrl, AnswerImageUrl = p.AnswerImageUrl,
                QuestionAudioUrl = p.QuestionAudioUrl, AnswerAudioUrl = p.AnswerAudioUrl,
                Explanation = p.Explanation, PairOrder = p.PairOrder
            });
        }
        game.NumberOfPairs = dto.Pairs.Count;
        _uow.MatchingGames.Update(game);
        await _uow.SaveChangesAsync();
        return await GetByIdAsync(id);
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var game = await _uow.MatchingGames.GetByIdAsync(id);
        if (game == null) return Result.Fail("Game not found");
        _uow.MatchingGames.Delete(game);
        await _uow.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result<MatchingGameStartDto>> StartSessionAsync(int gameId, string? userId)
    {
        var game = await _uow.MatchingGames.Query().Include(m => m.Pairs).FirstOrDefaultAsync(m => m.Id == gameId);
        if (game == null) return Result<MatchingGameStartDto>.Fail("Game not found");

        var session = new MatchingGameSession
        {
            UserId = userId, MatchingGameId = gameId, TotalPairs = game.NumberOfPairs, StartTime = DateTime.UtcNow
        };
        await _uow.MatchingGameSessions.AddAsync(session);
        await _uow.SaveChangesAsync();

        var shuffledPairs = game.Pairs.OrderBy(_ => Guid.NewGuid()).ToList();
        return Result<MatchingGameStartDto>.Ok(new MatchingGameStartDto
        {
            GameId = gameId, SessionId = session.Id, GameTitle = game.GameTitle,
            MatchingMode = game.MatchingMode, TimerMode = game.TimerMode,
            TimeLimitSeconds = game.TimeLimitSeconds, PointsPerMatch = game.PointsPerMatch,
            EnableHints = game.EnableHints, MaxHints = game.MaxHints,
            LeftItems = shuffledPairs.Select(p => new ShuffledItem { PairId = p.Id, Text = p.QuestionText, ImageUrl = p.QuestionImageUrl, AudioUrl = p.QuestionAudioUrl }).ToList(),
            RightItems = shuffledPairs.OrderBy(_ => Guid.NewGuid()).Select(p => new ShuffledItem { PairId = p.Id, Text = p.AnswerText, ImageUrl = p.AnswerImageUrl, AudioUrl = p.AnswerAudioUrl }).ToList()
        });
    }

    public async Task<Result<GameSessionResultDto>> SubmitSessionAsync(MatchingSubmitDto dto, string? userId)
    {
        var session = await _uow.MatchingGameSessions.Query()
            .Include(s => s.MatchingGame).FirstOrDefaultAsync(s => s.Id == dto.SessionId);
        if (session == null) return Result<GameSessionResultDto>.Fail("Session not found");

        int correct = 0, wrong = 0;
        foreach (var move in dto.Moves)
        {
            bool isCorrect = move.QuestionPairId == move.SelectedAnswerPairId;
            if (isCorrect) correct++; else wrong++;
            await _uow.MatchingAttempts.AddAsync(new MatchingAttempt
            {
                SessionId = dto.SessionId, QuestionPairId = move.QuestionPairId,
                SelectedAnswerPairId = move.SelectedAnswerPairId, IsCorrect = isCorrect, TimeSpentMs = move.TimeSpentMs
            });
        }

        var game = session.MatchingGame;
        int score = (correct * game.PointsPerMatch) - (wrong * game.WrongMatchPenalty);
        score = Math.Max(0, score);

        session.CorrectMatches = correct;
        session.WrongAttempts = wrong;
        session.TotalScore = score;
        session.HintsUsed = dto.HintsUsed;
        session.TimeSpentSeconds = dto.TimeSpentSeconds;
        session.EndTime = DateTime.UtcNow;
        session.IsCompleted = true;
        _uow.MatchingGameSessions.Update(session);
        await _uow.SaveChangesAsync();

        return Result<GameSessionResultDto>.Ok(new GameSessionResultDto
        {
            SessionId = session.Id, GameId = game.Id, GameType = "Matching",
            TotalScore = score, CorrectCount = correct, WrongCount = wrong,
            TimeSpentSeconds = dto.TimeSpentSeconds, HintsUsed = dto.HintsUsed,
            IsCompleted = true, PointsEarned = score
        });
    }

    private static MatchingGameDto MapToDto(MatchingGame m) => new()
    {
        Id = m.Id, GameTitle = m.GameTitle, Instructions = m.Instructions,
        GradeId = m.GradeId, SkillCategory = m.SkillCategory, NumberOfPairs = m.NumberOfPairs,
        MatchingMode = m.MatchingMode, TimerMode = m.TimerMode, TimeLimitSeconds = m.TimeLimitSeconds,
        DifficultyLevel = m.DifficultyLevel, PointsPerMatch = m.PointsPerMatch,
        EnableHints = m.EnableHints, MaxHints = m.MaxHints, ThumbnailUrl = m.ThumbnailUrl,
        Category = m.Category,
        Pairs = m.Pairs.OrderBy(p => p.PairOrder).Select(p => new MatchingGamePairDto
        {
            Id = p.Id, QuestionText = p.QuestionText, AnswerText = p.AnswerText,
            QuestionImageUrl = p.QuestionImageUrl, AnswerImageUrl = p.AnswerImageUrl,
            QuestionAudioUrl = p.QuestionAudioUrl, AnswerAudioUrl = p.AnswerAudioUrl,
            Explanation = p.Explanation, PairOrder = p.PairOrder
        }).ToList()
    };
}
