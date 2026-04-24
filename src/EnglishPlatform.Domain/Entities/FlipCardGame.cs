using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Flip card memory game configuration.
/// </summary>
public class FlipCardQuestion : BaseEntity
{
    public int Id { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public FlipCardGameMode GameMode { get; set; } = FlipCardGameMode.ClassicMemory;
    public int NumberOfPairs { get; set; } = 6;
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    public FlipCardTimerMode TimerMode { get; set; } = FlipCardTimerMode.CountUp;
    public int? TimeLimitSeconds { get; set; }
    public int PointsPerMatch { get; set; } = 100;
    public int MovePenalty { get; set; } = 5;
    public bool ShowHints { get; set; } = true;
    public int MaxHints { get; set; } = 2;
    public string UITheme { get; set; } = "modern";
    public string CardBackDesign { get; set; } = "pattern1";
    public string? CustomCardBackUrl { get; set; }
    public bool EnableAudio { get; set; } = true;
    public bool EnableExplanations { get; set; } = true;
    public string? Category { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ThumbnailUrl { get; set; }

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
    public virtual ICollection<FlipCardPair> Pairs { get; set; } = new List<FlipCardPair>();
    public virtual ICollection<FlipCardGameSession> Sessions { get; set; } = new List<FlipCardGameSession>();
}

/// <summary>
/// Card pair for flip card game.
/// </summary>
public class FlipCardPair
{
    public int Id { get; set; }
    public int FlipCardQuestionId { get; set; }
    public FlipCardContentType Card1Type { get; set; } = FlipCardContentType.Text;
    public string? Card1Text { get; set; }
    public string? Card1ImageUrl { get; set; }
    public string? Card1AudioUrl { get; set; }
    public FlipCardContentType Card2Type { get; set; } = FlipCardContentType.Text;
    public string? Card2Text { get; set; }
    public string? Card2ImageUrl { get; set; }
    public string? Card2AudioUrl { get; set; }
    public string? Explanation { get; set; }
    public int PairOrder { get; set; }
    public int DifficultyWeight { get; set; } = 5;

    // Navigation
    public virtual FlipCardQuestion FlipCardQuestion { get; set; } = null!;
}

/// <summary>
/// Flip card game play session.
/// </summary>
public class FlipCardGameSession : BaseEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public Guid? GuestSessionId { get; set; }
    public int FlipCardQuestionId { get; set; }
    public int TotalPairs { get; set; }
    public int MatchesFound { get; set; }
    public int TotalFlips { get; set; }
    public int WrongFlips { get; set; }
    public int HintsUsed { get; set; }
    public int TotalScore { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public bool IsCompleted { get; set; } = false;

    // Navigation
    public virtual ApplicationUser? User { get; set; }
    public virtual FlipCardQuestion FlipCardQuestion { get; set; } = null!;
    public virtual ICollection<FlipCardAttempt> Attempts { get; set; } = new List<FlipCardAttempt>();
}

/// <summary>
/// Individual card flip attempt.
/// </summary>
public class FlipCardAttempt
{
    public long Id { get; set; }
    public int SessionId { get; set; }
    public int Card1PairId { get; set; }
    public int Card2PairId { get; set; }
    public int Card1Position { get; set; }
    public int Card2Position { get; set; }
    public bool IsMatch { get; set; }
    public int PointsEarned { get; set; }
    public DateTime AttemptTimestamp { get; set; } = DateTime.UtcNow;
    public int TimeSpentMs { get; set; }

    // Navigation
    public virtual FlipCardGameSession Session { get; set; } = null!;
}
