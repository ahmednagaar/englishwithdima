namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Curriculum unit within a grade.
/// </summary>
public class Unit : BaseEntity
{
    public int Id { get; set; }
    public int GradeId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int UnitNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
