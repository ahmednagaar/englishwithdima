using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Application.DTOs.Games;

// ===== Matching Game DTOs =====
public class MatchingGameDto
{
    public int Id { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public int NumberOfPairs { get; set; }
    public MatchingMode MatchingMode { get; set; }
    public MatchingTimerMode TimerMode { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public int PointsPerMatch { get; set; }
    public bool EnableHints { get; set; }
    public int MaxHints { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? Category { get; set; }
    public List<MatchingGamePairDto> Pairs { get; set; } = new();
}

public class MatchingGamePairDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public string? QuestionAudioUrl { get; set; }
    public string AnswerText { get; set; } = string.Empty;
    public string? AnswerImageUrl { get; set; }
    public string? AnswerAudioUrl { get; set; }
    public string? Explanation { get; set; }
    public int PairOrder { get; set; }
}

public class CreateMatchingGameDto
{
    public string GameTitle { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public MatchingMode MatchingMode { get; set; } = MatchingMode.Both;
    public MatchingTimerMode TimerMode { get; set; } = MatchingTimerMode.CountUp;
    public int? TimeLimitSeconds { get; set; }
    public int PointsPerMatch { get; set; } = 100;
    public int WrongMatchPenalty { get; set; } = 5;
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    public bool EnableHints { get; set; } = true;
    public int MaxHints { get; set; } = 3;
    public string? Category { get; set; }
    public string? ThumbnailUrl { get; set; }
    public List<CreateMatchingGamePairDto> Pairs { get; set; } = new();
}

public class CreateMatchingGamePairDto
{
    public string QuestionText { get; set; } = string.Empty;
    public string? QuestionImageUrl { get; set; }
    public string? QuestionAudioUrl { get; set; }
    public ContentType QuestionType { get; set; } = ContentType.Text;
    public string AnswerText { get; set; } = string.Empty;
    public string? AnswerImageUrl { get; set; }
    public string? AnswerAudioUrl { get; set; }
    public ContentType AnswerType { get; set; } = ContentType.Text;
    public string? Explanation { get; set; }
    public int PairOrder { get; set; }
}

public class MatchingGameStartDto
{
    public int GameId { get; set; }
    public int SessionId { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public MatchingMode MatchingMode { get; set; }
    public MatchingTimerMode TimerMode { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public int PointsPerMatch { get; set; }
    public bool EnableHints { get; set; }
    public int MaxHints { get; set; }
    public List<ShuffledItem> LeftItems { get; set; } = new();
    public List<ShuffledItem> RightItems { get; set; } = new();
}

public class ShuffledItem
{
    public int PairId { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
}

public class MatchingSubmitDto
{
    public int SessionId { get; set; }
    public int GameId { get; set; }
    public List<MatchingMoveDto> Moves { get; set; } = new();
    public int TimeSpentSeconds { get; set; }
    public int HintsUsed { get; set; }
}

public class MatchingMoveDto
{
    public int QuestionPairId { get; set; }
    public int SelectedAnswerPairId { get; set; }
    public int TimeSpentMs { get; set; }
}

// ===== Wheel Game DTOs =====
public class WheelGameStartDto
{
    public int SessionId { get; set; }
    public int GradeId { get; set; }
    public List<WheelSegmentDto> Segments { get; set; } = new();
    public int TotalQuestions { get; set; }
}

public class WheelSegmentDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public WheelSegmentType SegmentType { get; set; }
    public int Value { get; set; }
}

public class WheelSpinResultDto
{
    public WheelSegmentDto Segment { get; set; } = null!;
    public WheelQuestionDto? Question { get; set; }
}

public class WheelQuestionDto
{
    public int Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public QuestionType QuestionType { get; set; }
    public string? AudioUrl { get; set; }
    public string? ImageUrl { get; set; }
    public List<string> Options { get; set; } = new();
    public int TimeLimit { get; set; }
    public string? HintText { get; set; }
    public int PointsValue { get; set; }
}

public class WheelAnswerDto
{
    public int SessionId { get; set; }
    public int QuestionId { get; set; }
    public string SelectedAnswer { get; set; } = string.Empty;
    public int TimeTakenSeconds { get; set; }
    public bool HintUsed { get; set; }
    public string SegmentLanded { get; set; } = string.Empty;
}

public class WheelAnswerResultDto
{
    public bool IsCorrect { get; set; }
    public string CorrectAnswer { get; set; } = string.Empty;
    public string? Explanation { get; set; }
    public int PointsEarned { get; set; }
    public int TotalScore { get; set; }
}

// ===== Drag & Drop DTOs =====
public class DragDropGameDto
{
    public int Id { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public int? TimeLimit { get; set; }
    public int PointsPerCorrectItem { get; set; }
    public string? ThumbnailUrl { get; set; }
    public List<DragDropZoneDto> Zones { get; set; } = new();
    public List<DragDropItemDto> Items { get; set; } = new();
}

public class DragDropZoneDto
{
    public int Id { get; set; }
    public string ZoneLabel { get; set; } = string.Empty;
    public string? ZoneColor { get; set; }
    public string? ImageUrl { get; set; }
}

public class DragDropItemDto
{
    public int Id { get; set; }
    public string ItemText { get; set; } = string.Empty;
    public string? ItemImageUrl { get; set; }
    public string? ItemAudioUrl { get; set; }
}

public class CreateDragDropGameDto
{
    public string GameTitle { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    public int? TimeLimit { get; set; }
    public int PointsPerCorrectItem { get; set; } = 10;
    public bool ShowImmediateFeedback { get; set; } = true;
    public string? ThumbnailUrl { get; set; }
    public List<CreateDragDropZoneDto> Zones { get; set; } = new();
    public List<CreateDragDropItemDto> Items { get; set; } = new();
}

public class CreateDragDropZoneDto
{
    public string ZoneLabel { get; set; } = string.Empty;
    public string? ZoneColor { get; set; }
    public string? ImageUrl { get; set; }
    public int ZoneOrder { get; set; }
}

public class CreateDragDropItemDto
{
    public string ItemText { get; set; } = string.Empty;
    public string? ItemImageUrl { get; set; }
    public string? ItemAudioUrl { get; set; }
    public int CorrectZoneIndex { get; set; }
    public string? Explanation { get; set; }
    public int ItemOrder { get; set; }
}

public class DragDropSubmitDto
{
    public int SessionId { get; set; }
    public int GameId { get; set; }
    public List<DragDropMoveDto> Moves { get; set; } = new();
    public int TimeSpentSeconds { get; set; }
}

public class DragDropMoveDto
{
    public int ItemId { get; set; }
    public int PlacedInZoneId { get; set; }
    public int TimeSpentMs { get; set; }
}

// ===== Flip Card DTOs =====
public class FlipCardGameDto
{
    public int Id { get; set; }
    public string GameTitle { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public FlipCardGameMode GameMode { get; set; }
    public int NumberOfPairs { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public FlipCardTimerMode TimerMode { get; set; }
    public int? TimeLimitSeconds { get; set; }
    public int PointsPerMatch { get; set; }
    public bool ShowHints { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string CardBackDesign { get; set; } = string.Empty;
    public List<FlipCardPairDto> Pairs { get; set; } = new();
}

public class FlipCardPairDto
{
    public int Id { get; set; }
    public string? Card1Text { get; set; }
    public string? Card1ImageUrl { get; set; }
    public string? Card1AudioUrl { get; set; }
    public string? Card2Text { get; set; }
    public string? Card2ImageUrl { get; set; }
    public string? Card2AudioUrl { get; set; }
    public string? Explanation { get; set; }
}

public class CreateFlipCardGameDto
{
    public string GameTitle { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public int GradeId { get; set; }
    public SkillCategory SkillCategory { get; set; }
    public ContentTopic? ContentTopic { get; set; }
    public FlipCardGameMode GameMode { get; set; } = FlipCardGameMode.ClassicMemory;
    public int NumberOfPairs { get; set; } = 6;
    public DifficultyLevel DifficultyLevel { get; set; } = DifficultyLevel.Medium;
    public FlipCardTimerMode TimerMode { get; set; } = FlipCardTimerMode.CountUp;
    public int? TimeLimitSeconds { get; set; }
    public int PointsPerMatch { get; set; } = 100;
    public bool ShowHints { get; set; } = true;
    public int MaxHints { get; set; } = 2;
    public string CardBackDesign { get; set; } = "pattern1";
    public string? ThumbnailUrl { get; set; }
    public List<CreateFlipCardPairDto> Pairs { get; set; } = new();
}

public class CreateFlipCardPairDto
{
    public FlipCardContentType Card1Type { get; set; } = FlipCardContentType.Text;
    public string? Card1Text { get; set; }
    public string? Card1ImageUrl { get; set; }
    public string? Card1AudioUrl { get; set; }
    public FlipCardContentType Card2Type { get; set; } = FlipCardContentType.Text;
    public string? Card2Text { get; set; }
    public string? Card2ImageUrl { get; set; }
    public string? Card2AudioUrl { get; set; }
    public string? Explanation { get; set; }
    public int PairOrder { get; set; }
}

public class FlipCardSubmitDto
{
    public int SessionId { get; set; }
    public int GameId { get; set; }
    public int MatchesFound { get; set; }
    public int TotalFlips { get; set; }
    public int WrongFlips { get; set; }
    public int HintsUsed { get; set; }
    public int TimeSpentSeconds { get; set; }
}

// ===== Shared Game Result DTO =====
public class GameSessionResultDto
{
    public int SessionId { get; set; }
    public int GameId { get; set; }
    public string GameType { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int CorrectCount { get; set; }
    public int WrongCount { get; set; }
    public int TimeSpentSeconds { get; set; }
    public int HintsUsed { get; set; }
    public bool IsCompleted { get; set; }
    public int PointsEarned { get; set; }
    public List<string> BadgesEarned { get; set; } = new();
}

public class GameFilterDto
{
    public int? GradeId { get; set; }
    public SkillCategory? SkillCategory { get; set; }
    public DifficultyLevel? DifficultyLevel { get; set; }
    public string? Category { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
