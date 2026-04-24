using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.Repositories.Interfaces;

namespace EnglishPlatform.Infrastructure.UnitOfWork;

/// <summary>
/// Unit of Work interface — single point of access for all repositories.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Specialized repositories
    ITestRepository Tests { get; }
    ILeaderboardRepository Leaderboard { get; }
    IStudentProgressRepository StudentProgress { get; }

    // Generic repositories
    IGenericRepository<Question> Questions { get; }
    IGenericRepository<QuestionOption> QuestionOptions { get; }
    IGenericRepository<SubQuestion> SubQuestions { get; }
    IGenericRepository<MatchingPair> QuestionMatchingPairs { get; }
    IGenericRepository<TestQuestion> TestQuestions { get; }
    IGenericRepository<TestAttempt> TestAttempts { get; }
    IGenericRepository<AttemptAnswer> AttemptAnswers { get; }
    IGenericRepository<Grade> Grades { get; }
    IGenericRepository<Unit> Units { get; }
    IGenericRepository<Lesson> Lessons { get; }

    // Game repositories
    IGenericRepository<MatchingGame> MatchingGames { get; }
    IGenericRepository<MatchingGamePair> MatchingGamePairs { get; }
    IGenericRepository<MatchingGameSession> MatchingGameSessions { get; }
    IGenericRepository<MatchingAttempt> MatchingAttempts { get; }
    IGenericRepository<WheelQuestion> WheelQuestions { get; }
    IGenericRepository<WheelSpinSegment> WheelSegments { get; }
    IGenericRepository<WheelGameSession> WheelGameSessions { get; }
    IGenericRepository<WheelQuestionAttempt> WheelQuestionAttempts { get; }
    IGenericRepository<DragDropQuestion> DragDropQuestions { get; }
    IGenericRepository<DragDropZone> DragDropZones { get; }
    IGenericRepository<DragDropItem> DragDropItems { get; }
    IGenericRepository<DragDropGameSession> DragDropGameSessions { get; }
    IGenericRepository<DragDropAttempt> DragDropAttempts { get; }
    IGenericRepository<FlipCardQuestion> FlipCardQuestions { get; }
    IGenericRepository<FlipCardPair> FlipCardPairs { get; }
    IGenericRepository<FlipCardGameSession> FlipCardGameSessions { get; }
    IGenericRepository<FlipCardAttempt> FlipCardAttempts { get; }

    // Gamification
    IGenericRepository<Badge> Badges { get; }
    IGenericRepository<UserBadge> UserBadges { get; }

    // Notifications
    IGenericRepository<Notification> Notifications { get; }
    IGenericRepository<UserNotification> UserNotifications { get; }

    // Booking & Contact
    IGenericRepository<BookingRequest> BookingRequests { get; }
    IGenericRepository<ContactMessage> ContactMessages { get; }

    // System
    IGenericRepository<AuditLog> AuditLogs { get; }
    IGenericRepository<SystemSetting> SystemSettings { get; }
    IGenericRepository<SubscriptionPlan> SubscriptionPlans { get; }
    IGenericRepository<UserSubscription> UserSubscriptions { get; }

    // Auth
    IGenericRepository<GuestSession> GuestSessions { get; }
    IGenericRepository<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync();
}
