using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.Data;
using EnglishPlatform.Infrastructure.Repositories.Implementations;
using EnglishPlatform.Infrastructure.Repositories.Interfaces;

namespace EnglishPlatform.Infrastructure.UnitOfWork;

/// <summary>
/// Unit of Work implementation — lazy-creates repositories on first access.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context) => _context = context;

    // Lazy backing fields
    private ITestRepository? _tests;
    private ILeaderboardRepository? _leaderboard;
    private IStudentProgressRepository? _studentProgress;
    private IGenericRepository<Question>? _questions;
    private IGenericRepository<QuestionOption>? _questionOptions;
    private IGenericRepository<SubQuestion>? _subQuestions;
    private IGenericRepository<MatchingPair>? _questionMatchingPairs;
    private IGenericRepository<TestQuestion>? _testQuestions;
    private IGenericRepository<TestAttempt>? _testAttempts;
    private IGenericRepository<AttemptAnswer>? _attemptAnswers;
    private IGenericRepository<Grade>? _grades;
    private IGenericRepository<Unit>? _units;
    private IGenericRepository<Lesson>? _lessons;
    private IGenericRepository<MatchingGame>? _matchingGames;
    private IGenericRepository<MatchingGamePair>? _matchingGamePairs;
    private IGenericRepository<MatchingGameSession>? _matchingGameSessions;
    private IGenericRepository<MatchingAttempt>? _matchingAttempts;
    private IGenericRepository<WheelQuestion>? _wheelQuestions;
    private IGenericRepository<WheelSpinSegment>? _wheelSegments;
    private IGenericRepository<WheelGameSession>? _wheelGameSessions;
    private IGenericRepository<WheelQuestionAttempt>? _wheelQuestionAttempts;
    private IGenericRepository<DragDropQuestion>? _dragDropQuestions;
    private IGenericRepository<DragDropZone>? _dragDropZones;
    private IGenericRepository<DragDropItem>? _dragDropItems;
    private IGenericRepository<DragDropGameSession>? _dragDropGameSessions;
    private IGenericRepository<DragDropAttempt>? _dragDropAttempts;
    private IGenericRepository<FlipCardQuestion>? _flipCardQuestions;
    private IGenericRepository<FlipCardPair>? _flipCardPairs;
    private IGenericRepository<FlipCardGameSession>? _flipCardGameSessions;
    private IGenericRepository<FlipCardAttempt>? _flipCardAttempts;
    private IGenericRepository<Badge>? _badges;
    private IGenericRepository<UserBadge>? _userBadges;
    private IGenericRepository<Notification>? _notifications;
    private IGenericRepository<UserNotification>? _userNotifications;
    private IGenericRepository<BookingRequest>? _bookingRequests;
    private IGenericRepository<ContactMessage>? _contactMessages;
    private IGenericRepository<AuditLog>? _auditLogs;
    private IGenericRepository<SystemSetting>? _systemSettings;
    private IGenericRepository<SubscriptionPlan>? _subscriptionPlans;
    private IGenericRepository<UserSubscription>? _userSubscriptions;
    private IGenericRepository<GuestSession>? _guestSessions;
    private IGenericRepository<RefreshToken>? _refreshTokens;

    // Property accessors
    public ITestRepository Tests => _tests ??= new TestRepository(_context);
    public ILeaderboardRepository Leaderboard => _leaderboard ??= new LeaderboardRepository(_context);
    public IStudentProgressRepository StudentProgress => _studentProgress ??= new StudentProgressRepository(_context);
    public IGenericRepository<Question> Questions => _questions ??= new GenericRepository<Question>(_context);
    public IGenericRepository<QuestionOption> QuestionOptions => _questionOptions ??= new GenericRepository<QuestionOption>(_context);
    public IGenericRepository<SubQuestion> SubQuestions => _subQuestions ??= new GenericRepository<SubQuestion>(_context);
    public IGenericRepository<MatchingPair> QuestionMatchingPairs => _questionMatchingPairs ??= new GenericRepository<MatchingPair>(_context);
    public IGenericRepository<TestQuestion> TestQuestions => _testQuestions ??= new GenericRepository<TestQuestion>(_context);
    public IGenericRepository<TestAttempt> TestAttempts => _testAttempts ??= new GenericRepository<TestAttempt>(_context);
    public IGenericRepository<AttemptAnswer> AttemptAnswers => _attemptAnswers ??= new GenericRepository<AttemptAnswer>(_context);
    public IGenericRepository<Grade> Grades => _grades ??= new GenericRepository<Grade>(_context);
    public IGenericRepository<Unit> Units => _units ??= new GenericRepository<Unit>(_context);
    public IGenericRepository<Lesson> Lessons => _lessons ??= new GenericRepository<Lesson>(_context);
    public IGenericRepository<MatchingGame> MatchingGames => _matchingGames ??= new GenericRepository<MatchingGame>(_context);
    public IGenericRepository<MatchingGamePair> MatchingGamePairs => _matchingGamePairs ??= new GenericRepository<MatchingGamePair>(_context);
    public IGenericRepository<MatchingGameSession> MatchingGameSessions => _matchingGameSessions ??= new GenericRepository<MatchingGameSession>(_context);
    public IGenericRepository<MatchingAttempt> MatchingAttempts => _matchingAttempts ??= new GenericRepository<MatchingAttempt>(_context);
    public IGenericRepository<WheelQuestion> WheelQuestions => _wheelQuestions ??= new GenericRepository<WheelQuestion>(_context);
    public IGenericRepository<WheelSpinSegment> WheelSegments => _wheelSegments ??= new GenericRepository<WheelSpinSegment>(_context);
    public IGenericRepository<WheelGameSession> WheelGameSessions => _wheelGameSessions ??= new GenericRepository<WheelGameSession>(_context);
    public IGenericRepository<WheelQuestionAttempt> WheelQuestionAttempts => _wheelQuestionAttempts ??= new GenericRepository<WheelQuestionAttempt>(_context);
    public IGenericRepository<DragDropQuestion> DragDropQuestions => _dragDropQuestions ??= new GenericRepository<DragDropQuestion>(_context);
    public IGenericRepository<DragDropZone> DragDropZones => _dragDropZones ??= new GenericRepository<DragDropZone>(_context);
    public IGenericRepository<DragDropItem> DragDropItems => _dragDropItems ??= new GenericRepository<DragDropItem>(_context);
    public IGenericRepository<DragDropGameSession> DragDropGameSessions => _dragDropGameSessions ??= new GenericRepository<DragDropGameSession>(_context);
    public IGenericRepository<DragDropAttempt> DragDropAttempts => _dragDropAttempts ??= new GenericRepository<DragDropAttempt>(_context);
    public IGenericRepository<FlipCardQuestion> FlipCardQuestions => _flipCardQuestions ??= new GenericRepository<FlipCardQuestion>(_context);
    public IGenericRepository<FlipCardPair> FlipCardPairs => _flipCardPairs ??= new GenericRepository<FlipCardPair>(_context);
    public IGenericRepository<FlipCardGameSession> FlipCardGameSessions => _flipCardGameSessions ??= new GenericRepository<FlipCardGameSession>(_context);
    public IGenericRepository<FlipCardAttempt> FlipCardAttempts => _flipCardAttempts ??= new GenericRepository<FlipCardAttempt>(_context);
    public IGenericRepository<Badge> Badges => _badges ??= new GenericRepository<Badge>(_context);
    public IGenericRepository<UserBadge> UserBadges => _userBadges ??= new GenericRepository<UserBadge>(_context);
    public IGenericRepository<Notification> Notifications => _notifications ??= new GenericRepository<Notification>(_context);
    public IGenericRepository<UserNotification> UserNotifications => _userNotifications ??= new GenericRepository<UserNotification>(_context);
    public IGenericRepository<BookingRequest> BookingRequests => _bookingRequests ??= new GenericRepository<BookingRequest>(_context);
    public IGenericRepository<ContactMessage> ContactMessages => _contactMessages ??= new GenericRepository<ContactMessage>(_context);
    public IGenericRepository<AuditLog> AuditLogs => _auditLogs ??= new GenericRepository<AuditLog>(_context);
    public IGenericRepository<SystemSetting> SystemSettings => _systemSettings ??= new GenericRepository<SystemSetting>(_context);
    public IGenericRepository<SubscriptionPlan> SubscriptionPlans => _subscriptionPlans ??= new GenericRepository<SubscriptionPlan>(_context);
    public IGenericRepository<UserSubscription> UserSubscriptions => _userSubscriptions ??= new GenericRepository<UserSubscription>(_context);
    public IGenericRepository<GuestSession> GuestSessions => _guestSessions ??= new GenericRepository<GuestSession>(_context);
    public IGenericRepository<RefreshToken> RefreshTokens => _refreshTokens ??= new GenericRepository<RefreshToken>(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
