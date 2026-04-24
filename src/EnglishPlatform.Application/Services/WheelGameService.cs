using EnglishPlatform.Application.DTOs.Games;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Domain.Enums;
using EnglishPlatform.Infrastructure.UnitOfWork;
using EnglishPlatform.Shared;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Application.Services;

public class WheelGameService : IWheelGameService
{
    private readonly IUnitOfWork _uow;
    public WheelGameService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<WheelGameStartDto>> StartSessionAsync(int gradeId, string? userId)
    {
        var segments = await _uow.WheelSegments.Query()
            .Where(s => s.GradeId == gradeId && s.IsActive)
            .OrderBy(s => s.DisplayOrder).ToListAsync();

        if (!segments.Any())
            return Result<WheelGameStartDto>.Fail("No wheel segments configured for this grade");

        var questionCount = await _uow.WheelQuestions.CountAsync(q => q.GradeId == gradeId && q.IsActive);

        var session = new WheelGameSession
        {
            UserId = userId, GradeId = gradeId, TotalQuestions = Math.Min(questionCount, 10), StartTime = DateTime.UtcNow
        };
        await _uow.WheelGameSessions.AddAsync(session);
        await _uow.SaveChangesAsync();

        return Result<WheelGameStartDto>.Ok(new WheelGameStartDto
        {
            SessionId = session.Id, GradeId = gradeId, TotalQuestions = session.TotalQuestions,
            Segments = segments.Select(s => new WheelSegmentDto
            {
                Id = s.Id, Label = s.Label, Color = s.Color, SegmentType = s.SegmentType, Value = s.Value
            }).ToList()
        });
    }

    public async Task<Result<WheelSpinResultDto>> SpinAsync(int sessionId)
    {
        var session = await _uow.WheelGameSessions.GetByIdAsync(sessionId);
        if (session == null) return Result<WheelSpinResultDto>.Fail("Session not found");

        var segments = await _uow.WheelSegments.Query()
            .Where(s => s.GradeId == session.GradeId && s.IsActive).ToListAsync();

        // Random segment
        var rng = new Random();
        var segment = segments[rng.Next(segments.Count)];

        WheelQuestionDto? question = null;
        if (segment.SegmentType == WheelSegmentType.Category || segment.SegmentType == WheelSegmentType.MysteryQuestion)
        {
            var answeredIds = await _uow.WheelQuestionAttempts.Query()
                .Where(a => a.SessionId == sessionId).Select(a => a.QuestionId).ToListAsync();

            var query = _uow.WheelQuestions.Query()
                .Where(q => q.GradeId == session.GradeId && q.IsActive && !answeredIds.Contains(q.Id));

            if (segment.SkillCategory.HasValue)
                query = query.Where(q => q.SkillCategory == segment.SkillCategory.Value);

            var q = await query.OrderBy(_ => Guid.NewGuid()).FirstOrDefaultAsync();
            if (q != null)
            {
                var options = new List<string> { q.CorrectAnswer };
                if (!string.IsNullOrEmpty(q.WrongAnswers))
                {
                    try { options.AddRange(System.Text.Json.JsonSerializer.Deserialize<List<string>>(q.WrongAnswers) ?? new()); }
                    catch { }
                }
                question = new WheelQuestionDto
                {
                    Id = q.Id, QuestionText = q.QuestionText, QuestionType = q.QuestionType,
                    AudioUrl = q.AudioUrl, ImageUrl = q.ImageUrl,
                    Options = options.OrderBy(_ => Guid.NewGuid()).ToList(),
                    TimeLimit = q.TimeLimit, HintText = q.HintText, PointsValue = q.PointsValue
                };
            }
        }

        return Result<WheelSpinResultDto>.Ok(new WheelSpinResultDto
        {
            Segment = new WheelSegmentDto { Id = segment.Id, Label = segment.Label, Color = segment.Color, SegmentType = segment.SegmentType, Value = segment.Value },
            Question = question
        });
    }

    public async Task<Result<WheelAnswerResultDto>> AnswerAsync(WheelAnswerDto dto)
    {
        var question = await _uow.WheelQuestions.GetByIdAsync(dto.QuestionId);
        if (question == null) return Result<WheelAnswerResultDto>.Fail("Question not found");

        bool isCorrect = dto.SelectedAnswer.Trim().Equals(question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
        int points = isCorrect ? question.PointsValue : 0;

        await _uow.WheelQuestionAttempts.AddAsync(new WheelQuestionAttempt
        {
            SessionId = dto.SessionId, QuestionId = dto.QuestionId,
            SegmentLanded = dto.SegmentLanded, SelectedAnswer = dto.SelectedAnswer,
            IsCorrect = isCorrect, PointsEarned = points,
            TimeTakenSeconds = dto.TimeTakenSeconds, HintUsed = dto.HintUsed
        });

        var session = await _uow.WheelGameSessions.GetByIdAsync(dto.SessionId);
        if (session != null)
        {
            session.QuestionsAnswered++;
            if (isCorrect) { session.CorrectAnswers++; session.TotalScore += points; }
            else session.WrongAnswers++;
            if (dto.HintUsed) session.HintsUsed++;
            _uow.WheelGameSessions.Update(session);
        }
        await _uow.SaveChangesAsync();

        return Result<WheelAnswerResultDto>.Ok(new WheelAnswerResultDto
        {
            IsCorrect = isCorrect, CorrectAnswer = question.CorrectAnswer,
            Explanation = question.Explanation, PointsEarned = points, TotalScore = session?.TotalScore ?? 0
        });
    }

    public async Task<Result<GameSessionResultDto>> EndSessionAsync(int sessionId)
    {
        var session = await _uow.WheelGameSessions.GetByIdAsync(sessionId);
        if (session == null) return Result<GameSessionResultDto>.Fail("Session not found");

        session.IsCompleted = true;
        session.EndTime = DateTime.UtcNow;
        session.TimeSpentSeconds = (int)(DateTime.UtcNow - session.StartTime).TotalSeconds;
        _uow.WheelGameSessions.Update(session);
        await _uow.SaveChangesAsync();

        return Result<GameSessionResultDto>.Ok(new GameSessionResultDto
        {
            SessionId = session.Id, GameId = session.GradeId, GameType = "Wheel",
            TotalScore = session.TotalScore, CorrectCount = session.CorrectAnswers,
            WrongCount = session.WrongAnswers, TimeSpentSeconds = session.TimeSpentSeconds,
            HintsUsed = session.HintsUsed, IsCompleted = true, PointsEarned = session.TotalScore
        });
    }
}
