namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// MCQ options for questions.
/// </summary>
public class QuestionOption : BaseEntity
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false;
    public int OrderIndex { get; set; }
    public string? ImageUrl { get; set; }

    // Navigation
    public virtual Question Question { get; set; } = null!;
}
