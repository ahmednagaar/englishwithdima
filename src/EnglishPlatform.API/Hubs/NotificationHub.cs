using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EnglishPlatform.API.Hubs;

/// <summary>
/// SignalR hub for real-time notifications, leaderboard updates, and badge awards.
/// </summary>
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// When a user connects, add them to their grade group for targeted notifications.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        var gradeId = Context.GetHttpContext()?.Request.Query["gradeId"].FirstOrDefault();

        if (!string.IsNullOrEmpty(gradeId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"grade-{gradeId}");
            _logger.LogInformation("User {UserId} joined grade group {GradeId}", userId, gradeId);
        }

        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("User {ConnectionId} disconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Join a specific grade's leaderboard channel.
    /// </summary>
    public async Task JoinGradeLeaderboard(int gradeId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"leaderboard-{gradeId}");
    }

    /// <summary>
    /// Leave a grade's leaderboard channel.
    /// </summary>
    public async Task LeaveGradeLeaderboard(int gradeId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"leaderboard-{gradeId}");
    }
}

/// <summary>
/// Service for sending real-time notifications from application services.
/// </summary>
public interface INotificationSender
{
    Task SendBadgeEarned(string userId, string badgeName, string badgeIcon, int bonusPoints);
    Task SendLeaderboardUpdate(int gradeId, string userId, string displayName, int totalPoints, int rank);
    Task SendTestCompleted(string userId, int testId, string testTitle, int score, int maxScore);
    Task SendNotification(string userId, string title, string body);
    Task BroadcastToGrade(int gradeId, string title, string body);
}

public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hub;

    public NotificationSender(IHubContext<NotificationHub> hub) => _hub = hub;

    public async Task SendBadgeEarned(string userId, string badgeName, string badgeIcon, int bonusPoints)
    {
        await _hub.Clients.Group($"user-{userId}").SendAsync("BadgeEarned", new
        {
            badgeName,
            badgeIcon,
            bonusPoints,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendLeaderboardUpdate(int gradeId, string userId, string displayName, int totalPoints, int rank)
    {
        await _hub.Clients.Group($"leaderboard-{gradeId}").SendAsync("LeaderboardUpdate", new
        {
            userId,
            displayName,
            totalPoints,
            rank,
            gradeId,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendTestCompleted(string userId, int testId, string testTitle, int score, int maxScore)
    {
        await _hub.Clients.Group($"user-{userId}").SendAsync("TestCompleted", new
        {
            testId,
            testTitle,
            score,
            maxScore,
            percentage = maxScore > 0 ? (int)(score * 100.0 / maxScore) : 0,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task SendNotification(string userId, string title, string body)
    {
        await _hub.Clients.Group($"user-{userId}").SendAsync("Notification", new
        {
            title,
            body,
            timestamp = DateTime.UtcNow
        });
    }

    public async Task BroadcastToGrade(int gradeId, string title, string body)
    {
        await _hub.Clients.Group($"grade-{gradeId}").SendAsync("Notification", new
        {
            title,
            body,
            timestamp = DateTime.UtcNow
        });
    }
}
