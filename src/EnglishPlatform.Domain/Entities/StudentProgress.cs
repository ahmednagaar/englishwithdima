namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Student progress tracking per grade.
/// </summary>
public class StudentProgress : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int GradeId { get; set; }
    public int TotalPoints { get; set; } = 0;
    public int TotalTestsTaken { get; set; } = 0;
    public int TotalGamesPlayed { get; set; } = 0;
    public decimal AverageScore { get; set; } = 0;
    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
    public DateTime? LastActivityAt { get; set; }

    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Grade Grade { get; set; } = null!;
}
