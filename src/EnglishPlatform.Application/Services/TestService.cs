using AutoMapper;
using EnglishPlatform.Application.DTOs.Questions;
using EnglishPlatform.Application.DTOs.Tests;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Domain.Enums;
using EnglishPlatform.Infrastructure.UnitOfWork;
using EnglishPlatform.Shared;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Application.Services;

public class TestService : ITestService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TestService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedList<TestDto>>> GetTestsAsync(TestFilterDto filter)
    {
        var query = _unitOfWork.Tests.Query()
            .Include(t => t.Grade)
            .Include(t => t.Unit)
            .Include(t => t.Lesson)
            .Include(t => t.TestQuestions)
            .AsQueryable();

        if (filter.GradeId.HasValue) query = query.Where(t => t.GradeId == filter.GradeId.Value);
        if (filter.UnitId.HasValue) query = query.Where(t => t.UnitId == filter.UnitId.Value);
        if (filter.LessonId.HasValue) query = query.Where(t => t.LessonId == filter.LessonId.Value);
        if (filter.TestType.HasValue) query = query.Where(t => t.TestType == filter.TestType.Value);
        if (filter.SkillCategory.HasValue) query = query.Where(t => t.SkillCategory == filter.SkillCategory.Value);
        if (filter.IsPublished.HasValue) query = query.Where(t => t.IsPublished == filter.IsPublished.Value);
        if (!string.IsNullOrEmpty(filter.SearchTerm))
            query = query.Where(t => t.TitleEn.Contains(filter.SearchTerm) || t.TitleAr.Contains(filter.SearchTerm));

        query = query.OrderByDescending(t => t.CreatedAt);

        var pagedList = await query
            .Select(t => _mapper.Map<TestDto>(t))
            .ToPagedListAsync(filter.PageNumber, filter.PageSize);

        return Result<PagedList<TestDto>>.Ok(pagedList);
    }

    public async Task<Result<TestDetailDto>> GetTestWithQuestionsAsync(int testId)
    {
        var test = await _unitOfWork.Tests.GetWithQuestionsAsync(testId);
        if (test == null)
            return Result<TestDetailDto>.Fail("الاختبار غير موجود / Test not found");

        return Result<TestDetailDto>.Ok(_mapper.Map<TestDetailDto>(test));
    }

    public async Task<Result<TestDto>> CreateTestAsync(CreateTestDto dto, string userId)
    {
        var test = _mapper.Map<Test>(dto);
        test.CreatedBy = userId;
        test.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Tests.AddAsync(test);
        await _unitOfWork.SaveChangesAsync();

        // Add questions if provided
        if (dto.QuestionIds?.Any() == true)
        {
            for (int i = 0; i < dto.QuestionIds.Count; i++)
            {
                await _unitOfWork.TestQuestions.AddAsync(new TestQuestion
                {
                    TestId = test.Id,
                    QuestionId = dto.QuestionIds[i],
                    OrderIndex = i + 1,
                    IsRequired = true
                });
            }
            await _unitOfWork.SaveChangesAsync();

            // Recalculate total points
            await RecalculateTestPoints(test.Id);
        }

        return Result<TestDto>.Ok(_mapper.Map<TestDto>(test));
    }

    public async Task<Result<TestDto>> UpdateTestAsync(int id, UpdateTestDto dto, string userId)
    {
        var test = await _unitOfWork.Tests.GetByIdAsync(id);
        if (test == null) return Result<TestDto>.Fail("Test not found");

        test.TitleAr = dto.TitleAr;
        test.TitleEn = dto.TitleEn;
        test.DescriptionAr = dto.DescriptionAr;
        test.DescriptionEn = dto.DescriptionEn;
        test.Instructions = dto.Instructions;
        test.GradeId = dto.GradeId;
        test.UnitId = dto.UnitId;
        test.LessonId = dto.LessonId;
        test.TestType = dto.TestType;
        test.SkillCategory = dto.SkillCategory;
        test.IsTimedTest = dto.IsTimedTest;
        test.TimeLimitMinutes = dto.TimeLimitMinutes;
        test.PassingScore = dto.PassingScore;
        test.ShuffleQuestions = dto.ShuffleQuestions;
        test.ShuffleOptions = dto.ShuffleOptions;
        test.ShowCorrectAnswers = dto.ShowCorrectAnswers;
        test.ShowExplanations = dto.ShowExplanations;
        test.AllowRetake = dto.AllowRetake;
        test.MaxRetakeCount = dto.MaxRetakeCount;
        test.AvailableFrom = dto.AvailableFrom;
        test.AvailableTo = dto.AvailableTo;
        test.UpdatedBy = userId;

        _unitOfWork.Tests.Update(test);
        await _unitOfWork.SaveChangesAsync();

        return Result<TestDto>.Ok(_mapper.Map<TestDto>(test));
    }

    public async Task<Result> DeleteTestAsync(int id)
    {
        var test = await _unitOfWork.Tests.GetByIdAsync(id);
        if (test == null) return Result.Fail("Test not found");

        _unitOfWork.Tests.Delete(test);
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    public async Task<Result> PublishTestAsync(int id, bool publish)
    {
        var test = await _unitOfWork.Tests.GetByIdAsync(id);
        if (test == null) return Result.Fail("Test not found");

        test.IsPublished = publish;
        _unitOfWork.Tests.Update(test);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok(publish ? "تم نشر الاختبار / Test published" : "تم إلغاء نشر الاختبار / Test unpublished");
    }

    public async Task<Result> AddQuestionsToTestAsync(int testId, List<int> questionIds)
    {
        var test = await _unitOfWork.Tests.GetWithQuestionsAsync(testId);
        if (test == null) return Result.Fail("Test not found");

        var maxOrder = test.TestQuestions.Any() ? test.TestQuestions.Max(tq => tq.OrderIndex) : 0;

        foreach (var qId in questionIds)
        {
            if (test.TestQuestions.Any(tq => tq.QuestionId == qId)) continue;

            maxOrder++;
            await _unitOfWork.TestQuestions.AddAsync(new TestQuestion
            {
                TestId = testId,
                QuestionId = qId,
                OrderIndex = maxOrder,
                IsRequired = true
            });
        }

        await _unitOfWork.SaveChangesAsync();
        await RecalculateTestPoints(testId);

        return Result.Ok();
    }

    public async Task<Result> RemoveQuestionsFromTestAsync(int testId, List<int> questionIds)
    {
        var testQuestions = await _unitOfWork.TestQuestions.FindAsync(
            tq => tq.TestId == testId && questionIds.Contains(tq.QuestionId));

        foreach (var tq in testQuestions)
            _unitOfWork.TestQuestions.Delete(tq);

        await _unitOfWork.SaveChangesAsync();
        await RecalculateTestPoints(testId);

        return Result.Ok();
    }

    public async Task<Result> ReorderQuestionsAsync(int testId, ReorderQuestionsDto dto)
    {
        foreach (var item in dto.Items)
        {
            var tq = (await _unitOfWork.TestQuestions.FindAsync(
                x => x.TestId == testId && x.QuestionId == item.QuestionId)).FirstOrDefault();
            if (tq != null)
            {
                tq.OrderIndex = item.OrderIndex;
                _unitOfWork.TestQuestions.Update(tq);
            }
        }
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }

    // ===== Test-Taking =====

    public async Task<Result<AttemptStartDto>> StartAttemptAsync(int testId, string userId)
    {
        var test = await _unitOfWork.Tests.GetWithQuestionsAsync(testId);
        if (test == null)
            return Result<AttemptStartDto>.Fail("الاختبار غير موجود / Test not found");

        if (!test.IsPublished)
            return Result<AttemptStartDto>.Fail("الاختبار غير منشور / Test is not published");

        // Check retry limit
        if (!await CanUserRetryAsync(testId, userId))
            return Result<AttemptStartDto>.Fail("لقد وصلت للحد الأقصى من المحاولات / Maximum retries reached");

        var attemptCount = await _unitOfWork.Tests.CountAttemptsAsync(testId, userId);

        var attempt = new TestAttempt
        {
            TestId = testId,
            UserId = userId,
            AttemptNumber = attemptCount + 1,
            StartedAt = DateTime.UtcNow,
            MaxPossibleScore = test.TotalPoints,
            Status = AttemptStatus.InProgress
        };

        await _unitOfWork.TestAttempts.AddAsync(attempt);
        await _unitOfWork.SaveChangesAsync();

        // Build questions (shuffled if configured)
        var questions = test.TestQuestions
            .OrderBy(tq => tq.OrderIndex)
            .Select(tq => tq.Question)
            .ToList();

        if (test.ShuffleQuestions)
            questions = questions.OrderBy(_ => Guid.NewGuid()).ToList();

        var questionDtos = _mapper.Map<List<QuestionDto>>(questions);

        // Remove correct answers from response (prevent cheating)
        foreach (var q in questionDtos)
        {
            q.CorrectAnswer = null;
            q.Explanation = null;
            foreach (var opt in q.Options)
                opt.IsCorrect = false;
        }

        if (test.ShuffleOptions)
        {
            foreach (var q in questionDtos)
                q.Options = q.Options.OrderBy(_ => Guid.NewGuid()).ToList();
        }

        return Result<AttemptStartDto>.Ok(new AttemptStartDto
        {
            AttemptId = attempt.Id,
            TestId = testId,
            TestTitle = test.TitleEn,
            TimeLimitMinutes = test.TimeLimitMinutes,
            IsTimedTest = test.IsTimedTest,
            StartedAt = attempt.StartedAt,
            Questions = questionDtos
        });
    }

    public async Task<Result<AttemptResultDto>> SubmitAttemptAsync(SubmitAttemptDto dto, string userId)
    {
        var attempt = await _unitOfWork.TestAttempts.Query()
            .Include(a => a.Test)
                .ThenInclude(t => t.TestQuestions)
                    .ThenInclude(tq => tq.Question)
                        .ThenInclude(q => q.QuestionOptions)
            .FirstOrDefaultAsync(a => a.Id == dto.AttemptId && a.UserId == userId);

        if (attempt == null)
            return Result<AttemptResultDto>.Fail("المحاولة غير موجودة / Attempt not found");

        if (attempt.Status == AttemptStatus.Completed)
            return Result<AttemptResultDto>.Fail("تم تقديم هذه المحاولة بالفعل / Attempt already submitted");

        var test = attempt.Test;
        var answerResults = new List<AnswerResultDto>();
        decimal totalPointsEarned = 0;
        int correct = 0, wrong = 0, skipped = 0;

        foreach (var answer in dto.Answers)
        {
            var question = test.TestQuestions
                .Select(tq => tq.Question)
                .FirstOrDefault(q => q.Id == answer.QuestionId);

            if (question == null) continue;

            bool isCorrect = GradeAnswer(question, answer);
            decimal points = isCorrect ? question.Points : 0;

            if (isCorrect) correct++;
            else if (string.IsNullOrEmpty(answer.AnswerText) && answer.SelectedOptionIds?.Any() != true) skipped++;
            else wrong++;

            totalPointsEarned += points;

            // Save individual answer
            await _unitOfWork.AttemptAnswers.AddAsync(new AttemptAnswer
            {
                AttemptId = attempt.Id,
                QuestionId = answer.QuestionId,
                AnswerText = answer.AnswerText,
                SelectedOptionIds = answer.SelectedOptionIds != null
                    ? System.Text.Json.JsonSerializer.Serialize(answer.SelectedOptionIds) : null,
                IsCorrect = isCorrect,
                PointsEarned = points,
                TimeSpentSeconds = answer.TimeSpentSeconds
            });

            answerResults.Add(new AnswerResultDto
            {
                QuestionId = question.Id,
                QuestionText = question.QuestionText,
                SelectedAnswer = answer.AnswerText ?? string.Join(",", answer.SelectedOptionIds ?? new()),
                CorrectAnswer = question.CorrectAnswer,
                IsCorrect = isCorrect,
                PointsEarned = points,
                Explanation = test.ShowExplanations ? question.Explanation : null,
                TimeSpentSeconds = answer.TimeSpentSeconds
            });
        }

        // Update attempt
        var maxScore = test.TotalPoints > 0 ? test.TotalPoints : 1;
        var percentage = (totalPointsEarned / maxScore) * 100;

        attempt.TotalScore = totalPointsEarned;
        attempt.MaxPossibleScore = maxScore;
        attempt.Percentage = Math.Round(percentage, 2);
        attempt.CorrectAnswers = correct;
        attempt.WrongAnswers = wrong;
        attempt.SkippedAnswers = skipped;
        attempt.TimeSpentSeconds = dto.Answers.Sum(a => a.TimeSpentSeconds);
        attempt.Passed = percentage >= test.PassingScore;
        attempt.Status = AttemptStatus.Completed;
        attempt.SubmittedAt = DateTime.UtcNow;

        _unitOfWork.TestAttempts.Update(attempt);
        await _unitOfWork.SaveChangesAsync();

        return Result<AttemptResultDto>.Ok(new AttemptResultDto
        {
            AttemptId = attempt.Id,
            TestId = test.Id,
            TestTitle = test.TitleEn,
            Score = totalPointsEarned,
            MaxScore = maxScore,
            Percentage = attempt.Percentage,
            CorrectAnswers = correct,
            WrongAnswers = wrong,
            SkippedAnswers = skipped,
            TimeSpentSeconds = attempt.TimeSpentSeconds,
            Passed = attempt.Passed,
            AttemptNumber = attempt.AttemptNumber,
            PointsEarned = (int)totalPointsEarned,
            AnswerDetails = test.ShowCorrectAnswers ? answerResults : new()
        });
    }

    public async Task<Result<List<AttemptSummaryDto>>> GetUserAttemptsAsync(int testId, string userId)
    {
        var attempts = await _unitOfWork.TestAttempts.Query()
            .Include(a => a.Test)
            .Where(a => a.TestId == testId && a.UserId == userId && a.Status == AttemptStatus.Completed)
            .OrderByDescending(a => a.SubmittedAt)
            .ToListAsync();

        return Result<List<AttemptSummaryDto>>.Ok(_mapper.Map<List<AttemptSummaryDto>>(attempts));
    }

    public async Task<Result<AttemptResultDto>> GetAttemptDetailAsync(int attemptId, string userId)
    {
        var attempt = await _unitOfWork.TestAttempts.Query()
            .Include(a => a.Test)
            .Include(a => a.AttemptAnswers)
                .ThenInclude(aa => aa.Question)
            .FirstOrDefaultAsync(a => a.Id == attemptId && a.UserId == userId);

        if (attempt == null)
            return Result<AttemptResultDto>.Fail("Attempt not found");

        var details = attempt.AttemptAnswers.Select(aa => new AnswerResultDto
        {
            QuestionId = aa.QuestionId,
            QuestionText = aa.Question.QuestionText,
            SelectedAnswer = aa.AnswerText,
            CorrectAnswer = attempt.Test.ShowCorrectAnswers ? aa.Question.CorrectAnswer : null,
            IsCorrect = aa.IsCorrect ?? false,
            PointsEarned = aa.PointsEarned,
            Explanation = attempt.Test.ShowExplanations ? aa.Question.Explanation : null,
            TimeSpentSeconds = aa.TimeSpentSeconds
        }).ToList();

        return Result<AttemptResultDto>.Ok(new AttemptResultDto
        {
            AttemptId = attempt.Id,
            TestId = attempt.TestId,
            TestTitle = attempt.Test.TitleEn,
            Score = attempt.TotalScore,
            MaxScore = attempt.MaxPossibleScore,
            Percentage = attempt.Percentage,
            CorrectAnswers = attempt.CorrectAnswers,
            WrongAnswers = attempt.WrongAnswers,
            SkippedAnswers = attempt.SkippedAnswers,
            TimeSpentSeconds = attempt.TimeSpentSeconds,
            Passed = attempt.Passed,
            AttemptNumber = attempt.AttemptNumber,
            PointsEarned = (int)attempt.TotalScore,
            AnswerDetails = details
        });
    }

    public async Task<bool> CanUserRetryAsync(int testId, string userId)
    {
        var test = await _unitOfWork.Tests.GetByIdAsync(testId);
        if (test == null) return false;
        if (!test.AllowRetake) return await _unitOfWork.Tests.CountAttemptsAsync(testId, userId) == 0;
        if (test.MaxRetakeCount == null) return true;
        return await _unitOfWork.Tests.CountAttemptsAsync(testId, userId) < test.MaxRetakeCount;
    }

    // ===== Private Helpers =====

    private bool GradeAnswer(Question question, AnswerSubmissionDto answer)
    {
        switch (question.QuestionType)
        {
            case Domain.Enums.QuestionType.MultipleChoice:
                if (answer.SelectedOptionIds?.Any() == true)
                {
                    var correctIds = question.QuestionOptions
                        .Where(o => o.IsCorrect)
                        .Select(o => o.Id)
                        .ToHashSet();
                    return correctIds.SetEquals(answer.SelectedOptionIds.ToHashSet());
                }
                return false;

            case Domain.Enums.QuestionType.TrueFalse:
            case Domain.Enums.QuestionType.FillInTheBlank:
            case Domain.Enums.QuestionType.SentenceReorder:
            case Domain.Enums.QuestionType.ErrorCorrection:
            case Domain.Enums.QuestionType.ClozeTest:
                return !string.IsNullOrEmpty(answer.AnswerText) &&
                       answer.AnswerText.Trim().Equals(question.CorrectAnswer?.Trim(),
                           StringComparison.OrdinalIgnoreCase);

            case Domain.Enums.QuestionType.Writing:
                // Writing requires manual grading — auto-pass for now
                return true;

            default:
                return !string.IsNullOrEmpty(answer.AnswerText) &&
                       answer.AnswerText.Trim().Equals(question.CorrectAnswer?.Trim(),
                           StringComparison.OrdinalIgnoreCase);
        }
    }

    private async Task RecalculateTestPoints(int testId)
    {
        var test = await _unitOfWork.Tests.GetWithQuestionsAsync(testId);
        if (test == null) return;

        test.TotalPoints = test.TestQuestions.Sum(tq => tq.Question.Points);
        _unitOfWork.Tests.Update(test);
        await _unitOfWork.SaveChangesAsync();
    }
}
