namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Matching pairs for Connect Lines question type.
/// </summary>
public class MatchingPair : BaseEntity
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string LeftText { get; set; } = string.Empty;
    public string RightText { get; set; } = string.Empty;
    public string? LeftImageUrl { get; set; }
    public string? RightImageUrl { get; set; }
    public int PairIndex { get; set; }

    // Navigation
    public virtual Question Question { get; set; } = null!;
}
