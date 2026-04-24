using EnglishPlatform.Application.DTOs.Questions;
using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Application.DTOs.Tests;

public class TestDto
{
    public int Id { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public string? GradeName { get; set; }
    public int? UnitId { get; set; }
    public string? UnitName { get; set; }
    public int? LessonId { get; set; }
    public string? LessonName { get; set; }
    public TestType TestType { get; set; }
    public SkillCategory? SkillCategory { get; set; }
    public bool IsTimedTest { get; set; }
    public int? TimeLimitMinutes { get; set; }
    public int PassingScore { get; set; }
    public int TotalPoints { get; set; }
    public int QuestionCount { get; set; }
    public bool IsPublished { get; set; }
    public bool AllowRetake { get; set; }
    public int? MaxRetakeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TestDetailDto : TestDto
{
    public bool ShuffleQuestions { get; set; }
    public bool ShuffleOptions { get; set; }
    public bool ShowCorrectAnswers { get; set; }
    public bool ShowExplanations { get; set; }
    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableTo { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}

public class CreateTestDto
{
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public int? UnitId { get; set; }
    public int? LessonId { get; set; }
    public TestType TestType { get; set; }
    public SkillCategory? SkillCategory { get; set; }
    public bool IsTimedTest { get; set; }
    public int? TimeLimitMinutes { get; set; }
    public int PassingScore { get; set; } = 60;
    public bool ShuffleQuestions { get; set; } = true;
    public bool ShuffleOptions { get; set; } = true;
    public bool ShowCorrectAnswers { get; set; } = true;
    public bool ShowExplanations { get; set; } = true;
    public bool AllowRetake { get; set; } = true;
    public int? MaxRetakeCount { get; set; }
    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableTo { get; set; }
    public List<int>? QuestionIds { get; set; }
}

public class UpdateTestDto : CreateTestDto
{
    public int Id { get; set; }
}

public class AddQuestionsToTestDto
{
    public List<int> QuestionIds { get; set; } = new();
}

public class ReorderQuestionsDto
{
    public List<QuestionOrderItem> Items { get; set; } = new();
}

public class QuestionOrderItem
{
    public int QuestionId { get; set; }
    public int OrderIndex { get; set; }
}

// ===== Test Taking DTOs =====
public class AttemptStartDto
{
    public int AttemptId { get; set; }
    public int TestId { get; set; }
    public string TestTitle { get; set; } = string.Empty;
    public int? TimeLimitMinutes { get; set; }
    public bool IsTimedTest { get; set; }
    public DateTime StartedAt { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();
}

public class SubmitAttemptDto
{
    public int AttemptId { get; set; }
    public int TestId { get; set; }
    public List<AnswerSubmissionDto> Answers { get; set; } = new();
}

public class AnswerSubmissionDto
{
    public int QuestionId { get; set; }
    public string? AnswerText { get; set; }
    public List<int>? SelectedOptionIds { get; set; }
    public int TimeSpentSeconds { get; set; }
}

public class AttemptResultDto
{
    public int AttemptId { get; set; }
    public int TestId { get; set; }
    public string TestTitle { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public decimal MaxScore { get; set; }
    public decimal Percentage { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int SkippedAnswers { get; set; }
    public int TimeSpentSeconds { get; set; }
    public bool Passed { get; set; }
    public int AttemptNumber { get; set; }
    public int PointsEarned { get; set; }
    public List<AnswerResultDto> AnswerDetails { get; set; } = new();
    public List<string> BadgesEarned { get; set; } = new();
}

public class AnswerResultDto
{
    public int QuestionId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? SelectedAnswer { get; set; }
    public string? CorrectAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public decimal PointsEarned { get; set; }
    public string? Explanation { get; set; }
    public int TimeSpentSeconds { get; set; }
}

public class AttemptSummaryDto
{
    public int AttemptId { get; set; }
    public int TestId { get; set; }
    public string TestTitle { get; set; } = string.Empty;
    public decimal Percentage { get; set; }
    public bool Passed { get; set; }
    public int AttemptNumber { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime SubmittedAt { get; set; }
}

public class TestFilterDto
{
    public int? GradeId { get; set; }
    public int? UnitId { get; set; }
    public int? LessonId { get; set; }
    public TestType? TestType { get; set; }
    public SkillCategory? SkillCategory { get; set; }
    public bool? IsPublished { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
