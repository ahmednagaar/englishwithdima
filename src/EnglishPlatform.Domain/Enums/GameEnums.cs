namespace EnglishPlatform.Domain.Enums;

// Matching Game enums
public enum MatchingMode
{
    ClickToClick = 1,
    DragDrop = 2,
    Both = 3
}

public enum MatchingTimerMode
{
    None = 1,
    CountUp = 2,
    Countdown = 3
}

public enum ContentType
{
    Text = 1,
    Image = 2,
    TextAndImage = 3,
    Audio = 4,
    Mixed = 5
}

// Wheel Game enums
public enum WheelSegmentType
{
    Category = 1,
    BonusPoints = 2,
    DoublePoints = 3,
    LosePoints = 4,
    MysteryQuestion = 5,
    FreePass = 6
}

// Flip Card enums
public enum FlipCardGameMode
{
    ClassicMemory = 1,
    MatchMode = 2,
    Both = 3
}

public enum FlipCardTimerMode
{
    None = 1,
    CountUp = 2,
    Countdown = 3
}

public enum FlipCardContentType
{
    Text = 1,
    Image = 2,
    Audio = 3,
    TextAndImage = 4,
    TextAndAudio = 5,
    Mixed = 6
}

// Badge Type enum
public enum BadgeType
{
    FirstTest = 1,
    PerfectScore = 2,
    Streak7 = 3,
    Streak30 = 4,
    Top3Grade = 5,
    Top1Grade = 6,
    GameMaster = 7,
    SpeedRunner = 8,
    Curious = 9,
    AllUnits = 10
}
