using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Drag & Drop question/exercise configuration.
/// </summary>
public class DragDropQuestion : BaseEntity
{
    public int Id { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int NumberOfZones { get; set; } = 3;
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    public int? TimeLimit { get; set; }
    public int PointsPerCorrectItem { get; set; } = 10;
    public bool ShowImmediateFeedback { get; set; } = true;
    public string UITheme { get; set; } = "modern";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ThumbnailUrl { get; set; }

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
    public virtual ICollection<DragDropZone> Zones { get; set; } = new List<DragDropZone>();
    public virtual ICollection<DragDropItem> Items { get; set; } = new List<DragDropItem>();
    public virtual ICollection<DragDropGameSession> Sessions { get; set; } = new List<DragDropGameSession>();
}

/// <summary>
/// Drop zone definition.
/// </summary>
public class DragDropZone
{
    public int Id { get; set; }
    public int DragDropQuestionId { get; set; }
    public string ZoneLabel { get; set; } = string.Empty;
    public string? ZoneColor { get; set; }
    public int ZoneOrder { get; set; }
    public string? ImageUrl { get; set; }
    public string? Description { get; set; }

    // Navigation
    public virtual DragDropQuestion DragDropQuestion { get; set; } = null!;
    public virtual ICollection<DragDropItem> CorrectItems { get; set; } = new List<DragDropItem>();
}

/// <summary>
/// Draggable item.
/// </summary>
public class DragDropItem
{
    public int Id { get; set; }
    public int DragDropQuestionId { get; set; }
    public string ItemText { get; set; } = string.Empty;
    public string? ItemImageUrl { get; set; }
    public string? ItemAudioUrl { get; set; }
    public int CorrectZoneId { get; set; }
    public string? Explanation { get; set; }
    public int ItemOrder { get; set; }

    // Navigation
    public virtual DragDropQuestion DragDropQuestion { get; set; } = null!;
    public virtual DragDropZone CorrectZone { get; set; } = null!;
}

/// <summary>
/// Drag & drop play session.
/// </summary>
public class DragDropGameSession : BaseEntity
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public Guid? GuestSessionId { get; set; }
    public int DragDropQuestionId { get; set; }
    public int TotalItems { get; set; }
    public int CorrectPlacements { get; set; }
    public int WrongPlacements { get; set; }
    public int HintsUsed { get; set; }
    public int TotalScore { get; set; }
    public int TimeSpentSeconds { get; set; }
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public DateTime? EndTime { get; set; }
    public bool IsCompleted { get; set; } = false;

    // Navigation
    public virtual ApplicationUser? User { get; set; }
    public virtual DragDropQuestion DragDropQuestion { get; set; } = null!;
    public virtual ICollection<DragDropAttempt> Attempts { get; set; } = new List<DragDropAttempt>();
}

/// <summary>
/// Individual item placement attempt.
/// </summary>
public class DragDropAttempt
{
    public long Id { get; set; }
    public int SessionId { get; set; }
    public int ItemId { get; set; }
    public int PlacedInZoneId { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime AttemptTimestamp { get; set; } = DateTime.UtcNow;
    public int TimeSpentMs { get; set; }

    // Navigation
    public virtual DragDropGameSession Session { get; set; } = null!;
    public virtual DragDropItem Item { get; set; } = null!;
    public virtual DragDropZone PlacedInZone { get; set; } = null!;
}
