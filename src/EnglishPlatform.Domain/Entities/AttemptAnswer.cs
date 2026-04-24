namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Individual answer within a test attempt.
/// </summary>
public class AttemptAnswer : BaseEntity
{
    public int Id { get; set; }
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public string? AnswerText { get; set; }
    public string? SelectedOptionIds { get; set; }  // JSON array
    public bool? IsCorrect { get; set; }
    public decimal PointsEarned { get; set; }
    public int TimeSpentSeconds { get; set; }

    // Navigation
    public virtual TestAttempt Attempt { get; set; } = null!;
    public virtual Question Question { get; set; } = null!;
}
