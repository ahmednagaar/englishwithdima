namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Individual lesson within a unit.
/// </summary>
public class Lesson : BaseEntity
{
    public int Id { get; set; }
    public int UnitId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int LessonNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Unit Unit { get; set; } = null!;
}
