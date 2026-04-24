namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// School grade level (Grades 1-6 Primary, 1-3 Preparatory = 9 levels total).
/// </summary>
public class Grade : BaseEntity
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Level { get; set; }
    public string SchoolType { get; set; } = "Primary";
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
    public virtual ICollection<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
