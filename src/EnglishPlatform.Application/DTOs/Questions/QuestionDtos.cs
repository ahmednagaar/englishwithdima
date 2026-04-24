using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Application.DTOs.Questions;

public class QuestionDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? InstructionAr { get; set; }
    public QuestionType QuestionType { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public int GradeId { get; set; }
    public TestType? TestType { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? PassageText { get; set; }
    public string? CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    public string? HintText { get; set; }
    public string? Tags { get; set; }
    public int Points { get; set; }
    public int? EstimatedTimeMinutes { get; set; }
    public List<QuestionOptionDto> Options { get; set; } = new();
    public List<MatchingPairDto> MatchingPairs { get; set; } = new();
    public List<SubQuestionDto> SubQuestions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public class CreateQuestionDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string? InstructionAr { get; set; }
    public QuestionType QuestionType { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public int GradeId { get; set; }
    public TestType? TestType { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? PassageText { get; set; }
    public string? CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
    public string? HintText { get; set; }
    public string? Tags { get; set; }
    public int Points { get; set; } = 10;
    public int? EstimatedTimeMinutes { get; set; }
    public List<CreateQuestionOptionDto>? Options { get; set; }
    public List<CreateMatchingPairDto>? MatchingPairs { get; set; }
    public List<CreateSubQuestionDto>? SubQuestions { get; set; }
}

public class UpdateQuestionDto : CreateQuestionDto
{
    public int Id { get; set; }
}

public class QuestionOptionDto
{
    public int Id { get; set; }
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
    public string? ImageUrl { get; set; }
}

public class CreateQuestionOptionDto
{
    public string OptionText { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
    public int OrderIndex { get; set; }
    public string? ImageUrl { get; set; }
}

public class MatchingPairDto
{
    public int Id { get; set; }
    public string LeftText { get; set; } = string.Empty;
    public string RightText { get; set; } = string.Empty;
    public string? LeftImageUrl { get; set; }
    public string? RightImageUrl { get; set; }
    public int PairIndex { get; set; }
}

public class CreateMatchingPairDto
{
    public string LeftText { get; set; } = string.Empty;
    public string RightText { get; set; } = string.Empty;
    public string? LeftImageUrl { get; set; }
    public string? RightImageUrl { get; set; }
    public int PairIndex { get; set; }
}

public class SubQuestionDto
{
    public int Id { get; set; }
    public int OrderIndex { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string? Options { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int Points { get; set; }
}

public class CreateSubQuestionDto
{
    public int OrderIndex { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string? Options { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int Points { get; set; } = 10;
}

public class QuestionFilterDto
{
    public int? GradeId { get; set; }
    public QuestionType? QuestionType { get; set; }
    public DifficultyLevel? DifficultyLevel { get; set; }
    public SkillCategory? SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public TestType? TestType { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
