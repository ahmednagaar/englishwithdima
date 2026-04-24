using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Test (exam) entity — combines both specs' features.
/// </summary>
public class Test : BaseEntity
{
    public int Id { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public int? UnitId { get; set; }
    public int? LessonId { get; set; }
    public TestType TestType { get; set; }
    public SkillCategory? SkillCategory { get; set; }

    // Timer settings
    public bool IsTimedTest { get; set; } = false;
    public int? TimeLimitMinutes { get; set; }

    // Scoring
    public int PassingScore { get; set; } = 60;
    public int TotalPoints { get; set; }

    // Options
    public bool ShuffleQuestions { get; set; } = true;
    public bool ShuffleOptions { get; set; } = true;
    public bool ShowCorrectAnswers { get; set; } = true;
    public bool ShowExplanations { get; set; } = true;
    public bool AllowRetake { get; set; } = true;
    public int? MaxRetakeCount { get; set; }

    // Publishing
    public bool IsPublished { get; set; } = false;
    public DateTime? AvailableFrom { get; set; }
    public DateTime? AvailableTo { get; set; }

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
    public virtual Unit? Unit { get; set; }
    public virtual Lesson? Lesson { get; set; }
    public virtual ICollection<TestQuestion> TestQuestions { get; set; } = new List<TestQuestion>();
    public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
}
