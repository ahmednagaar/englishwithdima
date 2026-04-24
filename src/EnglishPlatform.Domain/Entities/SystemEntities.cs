using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Admin action audit trail.
/// </summary>
public class AuditLog
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;       // Create, Update, Delete, etc.
    public string EntityType { get; set; } = string.Empty;    // Question, Test, Game, etc.
    public string? EntityId { get; set; }
    public string? Details { get; set; }                       // JSON with before/after values
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Key-value system settings.
/// </summary>
public class SystemSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
}

/// <summary>
/// Subscription plan definition (future-ready).
/// </summary>
public class SubscriptionPlan : BaseEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal PriceEGP { get; set; }
    public int DurationDays { get; set; }
    public string? Features { get; set; }              // JSON array of feature keys
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}

/// <summary>
/// User subscription instance.
/// </summary>
public class UserSubscription : BaseEntity
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int PlanId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public string? PaymentReference { get; set; }

    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual SubscriptionPlan Plan { get; set; } = null!;
}
