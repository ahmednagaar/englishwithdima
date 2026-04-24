using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Student's attempt at a test, tracking score and detailed answers.
/// </summary>
public class TestAttempt : BaseEntity
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public string? UserId { get; set; }
    public Guid? GuestSessionId { get; set; }
    public int AttemptNumber { get; set; } = 1;
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? SubmittedAt { get; set; }
    public decimal TotalScore { get; set; }
    public decimal MaxPossibleScore { get; set; }
    public decimal Percentage { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int SkippedAnswers { get; set; }
    public int TimeSpentSeconds { get; set; }
    public bool Passed { get; set; }
    public AttemptStatus Status { get; set; } = AttemptStatus.InProgress;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }

    // Navigation
    public virtual Test Test { get; set; } = null!;
    public virtual ApplicationUser? User { get; set; }
    public virtual ICollection<AttemptAnswer> AttemptAnswers { get; set; } = new List<AttemptAnswer>();
}
