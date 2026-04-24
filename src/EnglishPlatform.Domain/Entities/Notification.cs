using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Notification sent by admin.
/// </summary>
public class Notification : BaseEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public NotificationTargetType TargetType { get; set; }
    public int? TargetGradeId { get; set; }
    public string? TargetUserId { get; set; }
    public DateTime? ScheduledAt { get; set; }

    // Navigation
    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
}

/// <summary>
/// Per-user notification status.
/// </summary>
public class UserNotification
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int NotificationId { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Notification Notification { get; set; } = null!;
}
