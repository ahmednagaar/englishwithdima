using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Wheel game question.
/// </summary>
public class WheelQuestion : BaseEntity
{
    public int Id { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? WrongAnswers { get; set; }         // JSON array for MCQ
    public string? AudioUrl { get; set; }
    public string? ImageUrl { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    public int PointsValue { get; set; } = 10;
    public int TimeLimit { get; set; } = 30;
    public string? HintText { get; set; }
    public string? Explanation { get; set; }
    public string? CategoryTag { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
}

/// <summary>
/// Wheel segment configuration (visual + behavior).
/// </summary>
public class WheelSpinSegment : BaseEntity
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Color { get; set; } = "#1A7FBF";
    public WheelSegmentType SegmentType { get; set; }
    public int Value { get; set; }
    public int GradeId { get; set; }
    public SkillCategory? SkillCategory { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
}

/// <summary>
/// Wheel game session.
/// </summary>
public class WheelGameSession : BaseEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public Guid? GuestSessionId { get; set; }
    public int GradeId { get; set; }
    public SkillCategory? SkillCategory { get; set; }
    public int TotalQuestions { get; set; }
    public int QuestionsAnswered { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int TotalScore { get; set; }
    public int HintsUsed { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public bool IsCompleted { get; set; } = false;
    public int TimeSpentSeconds { get; set; }
    public string? SessionData { get; set; }          // JSON: spin history, effects

    // Navigation
    public virtual ApplicationUser? User { get; set; }
    public virtual Grade Grade { get; set; } = null!;
    public virtual ICollection<WheelQuestionAttempt> Attempts { get; set; } = new List<WheelQuestionAttempt>();
}

/// <summary>
/// Individual wheel spin + answer attempt.
/// </summary>
public class WheelQuestionAttempt
{
    public long Id { get; set; }
    public int SessionId { get; set; }
    public int QuestionId { get; set; }
    public string? SegmentLanded { get; set; }
    public string? SelectedAnswer { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public int TimeTakenSeconds { get; set; }
    public bool HintUsed { get; set; }
    public DateTime AttemptTimestamp { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual WheelGameSession Session { get; set; } = null!;
    public virtual WheelQuestion Question { get; set; } = null!;
}
