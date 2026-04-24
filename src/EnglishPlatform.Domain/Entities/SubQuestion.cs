using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Sub-question for Passage/Reading Comprehension questions.
/// </summary>
public class SubQuestion : BaseEntity
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public int OrderIndex { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string? Options { get; set; }           // JSON for MCQ sub-questions
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int Points { get; set; } = 10;

    // Navigation
    public virtual Question Question { get; set; } = null!;
}
