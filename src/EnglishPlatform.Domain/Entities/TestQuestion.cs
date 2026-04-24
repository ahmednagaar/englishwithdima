namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Many-to-many junction between Test and Question with ordering.
/// </summary>
public class TestQuestion : BaseEntity
{
    public int Id { get; set; }
    public int TestId { get; set; }
    public int QuestionId { get; set; }
    public int OrderIndex { get; set; }
    public bool IsRequired { get; set; } = true;

    // Navigation
    public virtual Test Test { get; set; } = null!;
    public virtual Question Question { get; set; } = null!;
}
