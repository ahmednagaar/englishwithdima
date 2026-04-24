namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Guest sessions for anonymous test/game access.
/// </summary>
public class GuestSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string DisplayName { get; set; } = string.Empty;
    public int GradeId { get; set; }
    public string SessionToken { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
}
