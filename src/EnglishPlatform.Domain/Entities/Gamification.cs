using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Badge / Achievement definition.
/// </summary>
public class Badge : BaseEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? IconUrl { get; set; }
    public string Icon { get; set; } = "🏆";
    public BadgeType BadgeType { get; set; }
    public int? PointThreshold { get; set; }
    public int BonusPoints { get; set; } = 10;
    public string? CriteriaType { get; set; }
    public int? CriteriaValue { get; set; }
    public string? CriteriaSkill { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}

/// <summary>
/// Student-Badge junction (earned badges).
/// </summary>
public class UserBadge
{
    public string UserId { get; set; } = string.Empty;
    public int BadgeId { get; set; }
    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Badge Badge { get; set; } = null!;
}

/// <summary>
/// Leaderboard entry (materialized, refreshed by Hangfire).
/// </summary>
public class LeaderboardEntry : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int GradeId { get; set; }
    public int TotalPoints { get; set; }
    public int Rank { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }

    // Navigation
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Grade Grade { get; set; } = null!;
}
