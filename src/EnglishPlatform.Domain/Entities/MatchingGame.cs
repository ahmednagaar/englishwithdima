using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Matching game configuration.
/// </summary>
public class MatchingGame : BaseEntity
{
    public int Id { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public int NumberOfPairs { get; set; } = 6;
    public MatchingMode MatchingMode { get; set; } = MatchingMode.Both;
    public string UITheme { get; set; } = "modern";
    public bool ShowConnectingLines { get; set; } = true;
    public bool EnableAudio { get; set; } = true;
    public bool EnableHints { get; set; } = true;
    public int MaxHints { get; set; } = 3;
    public MatchingTimerMode TimerMode { get; set; } = MatchingTimerMode.CountUp;
    public int? TimeLimitSeconds { get; set; }
    public int PointsPerMatch { get; set; } = 100;
    public int WrongMatchPenalty { get; set; } = 5;
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    public string? Category { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ThumbnailUrl { get; set; }

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
    public virtual ICollection<MatchingGamePair> Pairs { get; set; } = new List<MatchingGamePair>();
    public virtual ICollection<MatchingGameSession> Sessions { get; set; } = new List<MatchingGameSession>();
}

/// <summary>
/// Content pair for matching game.
/// </summary>
public class MatchingGamePair : BaseEntity
{
    public int Id { get; set; }
    public int MatchingGameId { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public string? QuestionAudioUrl { get; set; }
    public ContentType QuestionType { get; set; } = ContentType.Text;
    public string AnswerText { get; set; } = string.Empty;
    public string? AnswerImageUrl { get; set; }
    public string? AnswerAudioUrl { get; set; }
    public ContentType AnswerType { get; set; } = ContentType.Text;
    public string? Explanation { get; set; }
    public int PairOrder { get; set; }

    // Navigation
    public virtual MatchingGame MatchingGame { get; set; } = null!;
}

/// <summary>
/// Student session for matching game.
/// </summary>
public class MatchingGameSession : BaseEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public Guid? GuestSessionId { get; set; }
    public int MatchingGameId { get; set; }
    public int TotalPairs { get; set; }
    public int CorrectMatches { get; set; }
    public int WrongAttempts { get; set; }
    public int HintsUsed { get; set; }
    public int TotalScore { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public bool IsCompleted { get; set; } = false;

    // Navigation
    public virtual ApplicationUser? User { get; set; }
    public virtual MatchingGame MatchingGame { get; set; } = null!;
    public virtual ICollection<MatchingAttempt> Attempts { get; set; } = new List<MatchingAttempt>();
}

/// <summary>
/// Individual match attempt within a session.
/// </summary>
public class MatchingAttempt
{
    public long Id { get; set; }
    public int SessionId { get; set; }
    public int QuestionPairId { get; set; }
    public int SelectedAnswerPairId { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime AttemptTimestamp { get; set; } = DateTime.UtcNow;
    public int TimeSpentMs { get; set; }

    // Navigation
    public virtual MatchingGameSession Session { get; set; } = null!;
}
