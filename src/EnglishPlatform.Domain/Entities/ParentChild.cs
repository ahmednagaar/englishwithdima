namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Links parent users to their children (students).
/// </summary>
public class ParentChild
{
    public string ParentId { get; set; } = string.Empty;
    public string ChildId { get; set; } = string.Empty;
    public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public virtual ApplicationUser Parent { get; set; } = null!;
    public virtual ApplicationUser Child { get; set; } = null!;
}
