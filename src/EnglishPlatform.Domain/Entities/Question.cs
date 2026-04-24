using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Question bank entity supporting all 11 question types.
/// Includes media support, skill categorization, and passage sub-questions.
/// </summary>
public class Question : BaseEntity
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? InstructionAr { get; set; }
    public QuestionType QuestionType { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public int GradeId { get; set; }
    public TestType? TestType { get; set; }

    // Media
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? VideoUrl { get; set; }

    // Content
    public string? PassageText { get; set; }
    public string? Options { get; set; }         // JSON: ["option1","option2","option3","option4"]
    public string? CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    public string? HintText { get; set; }
    public string? Tags { get; set; }             // JSON array: ["grammar","present-tense"]

    // Settings
    public int Points { get; set; } = 10;
    public int? EstimatedTimeMinutes { get; set; }
    public int OrderIndex { get; set; }

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
    public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new List<QuestionOption>();
    public virtual ICollection<MatchingPair> MatchingPairs { get; set; } = new List<MatchingPair>();
    public virtual ICollection<SubQuestion> SubQuestions { get; set; } = new List<SubQuestion>();
    public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
}
