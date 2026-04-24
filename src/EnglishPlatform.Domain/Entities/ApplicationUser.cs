using Microsoft.AspNetCore.Identity;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Extended ASP.NET Identity user with platform-specific fields.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int? GradeId { get; set; }
    public string Role { get; set; } = "Student";
    public bool IsGuest { get; set; } = false;
    public string? FacebookId { get; set; }
    public string? StudentCode { get; set; }
    public string? PinHash { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    public string PreferredLanguage { get; set; } = "ar";

    // Navigation properties
    public virtual Grade? Grade { get; set; }
    public virtual ICollection<ParentChild> ParentLinks { get; set; } = new List<ParentChild>();
    public virtual ICollection<ParentChild> ChildLinks { get; set; } = new List<ParentChild>();
    public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    public virtual ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
    public virtual ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
    public virtual StudentProgress? StudentProgress { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
