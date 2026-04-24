using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Domain.Enums;
using EnglishPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.API.Jobs;

/// <summary>
/// Hangfire background jobs for the platform.
/// </summary>
public class BackgroundJobs
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BackgroundJobs> _logger;

    public BackgroundJobs(IServiceProvider serviceProvider, ILogger<BackgroundJobs> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Refresh the materialized leaderboard table for all grades.
    /// Runs every 15 minutes via Hangfire recurring job.
    /// </summary>
    public async Task RefreshLeaderboardAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _logger.LogInformation("Starting leaderboard refresh...");

        var grades = await db.Set<Grade>().Where(g => g.IsActive).ToListAsync();

        foreach (var grade in grades)
        {
            // Calculate total points per student for this grade from StudentProgress
            var studentScores = await db.Set<StudentProgress>()
                .Where(sp => sp.GradeId == grade.Id)
                .OrderByDescending(sp => sp.TotalPoints)
                .Take(50) // Top 50 per grade
                .Select(sp => new
                {
                    sp.UserId,
                    sp.TotalPoints,
                    sp.User.FirstName,
                    sp.User.LastName,
                    sp.User.AvatarUrl
                })
                .ToListAsync();

            // Remove old leaderboard entries for this grade
            var oldEntries = await db.Set<LeaderboardEntry>()
                .Where(l => l.GradeId == grade.Id)
                .ToListAsync();
            db.Set<LeaderboardEntry>().RemoveRange(oldEntries);

            // Insert new ranked entries
            int rank = 1;
            foreach (var student in studentScores)
            {
                db.Set<LeaderboardEntry>().Add(new LeaderboardEntry
                {
                    UserId = student.UserId,
                    GradeId = grade.Id,
                    TotalPoints = student.TotalPoints,
                    Rank = rank++,
                    DisplayName = $"{student.FirstName} {student.LastName}".Trim(),
                    AvatarUrl = student.AvatarUrl
                });
            }
        }

        await db.SaveChangesAsync();
        _logger.LogInformation("Leaderboard refresh completed for {Count} grades", grades.Count);
    }

    /// <summary>
    /// Cleanup expired guest sessions (older than 24 hours).
    /// Runs daily via Hangfire recurring job.
    /// </summary>
    public async Task CleanupGuestSessionsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var cutoff = DateTime.UtcNow.AddHours(-24);
        var expired = await db.Set<GuestSession>()
            .Where(g => g.ExpiresAt < cutoff)
            .ToListAsync();

        if (expired.Any())
        {
            db.Set<GuestSession>().RemoveRange(expired);
            await db.SaveChangesAsync();
            _logger.LogInformation("Cleaned up {Count} expired guest sessions", expired.Count);
        }
    }

    /// <summary>
    /// Auto-complete timed test attempts that exceeded their time limit.
    /// Runs every 5 minutes via Hangfire recurring job.
    /// </summary>
    public async Task ExpireOverdueTestAttemptsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var overdueAttempts = await db.Set<TestAttempt>()
            .Include(a => a.Test)
            .Where(a => a.Status == AttemptStatus.InProgress
                        && a.Test.IsTimedTest
                        && a.Test.TimeLimitMinutes.HasValue
                        && a.StartedAt.AddMinutes(a.Test.TimeLimitMinutes!.Value + 2) < DateTime.UtcNow) // 2 min grace
            .ToListAsync();

        foreach (var attempt in overdueAttempts)
        {
            attempt.Status = AttemptStatus.Abandoned;
            attempt.SubmittedAt = DateTime.UtcNow;
            // Score remains 0 (no answers submitted)
            _logger.LogWarning("Auto-expired test attempt {AttemptId} for test {TestId}",
                attempt.Id, attempt.TestId);
        }

        if (overdueAttempts.Any())
        {
            await db.SaveChangesAsync();
            _logger.LogInformation("Expired {Count} overdue test attempts", overdueAttempts.Count);
        }
    }

    /// <summary>
    /// Evaluate and award badges to students based on their activity.
    /// Runs every 30 minutes via Hangfire recurring job.
    /// </summary>
    public async Task EvaluateBadgesAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var badges = await db.Set<Badge>().Where(b => b.IsActive).ToListAsync();
        var allProgress = await db.Set<StudentProgress>().ToListAsync();
        var existingBadges = await db.Set<UserBadge>().ToListAsync();

        int awarded = 0;

        foreach (var progress in allProgress)
        {
            foreach (var badge in badges)
            {
                // Skip if already earned
                if (existingBadges.Any(ub => ub.UserId == progress.UserId && ub.BadgeId == badge.Id))
                    continue;

                bool earned = false;

                // Evaluate criteria
                if (badge.CriteriaType == "TotalPoints" && badge.CriteriaValue.HasValue)
                    earned = progress.TotalPoints >= badge.CriteriaValue.Value;
                else if (badge.CriteriaType == "TestsTaken" && badge.CriteriaValue.HasValue)
                    earned = progress.TotalTestsTaken >= badge.CriteriaValue.Value;
                else if (badge.CriteriaType == "GamesPlayed" && badge.CriteriaValue.HasValue)
                    earned = progress.TotalGamesPlayed >= badge.CriteriaValue.Value;
                else if (badge.CriteriaType == "Streak" && badge.CriteriaValue.HasValue)
                    earned = progress.CurrentStreak >= badge.CriteriaValue.Value;
                else if (badge.PointThreshold.HasValue)
                    earned = progress.TotalPoints >= badge.PointThreshold.Value;

                if (earned)
                {
                    db.Set<UserBadge>().Add(new UserBadge
                    {
                        UserId = progress.UserId,
                        BadgeId = badge.Id,
                        EarnedAt = DateTime.UtcNow
                    });

                    // Add bonus points
                    progress.TotalPoints += badge.BonusPoints;
                    awarded++;
                }
            }
        }

        if (awarded > 0)
        {
            await db.SaveChangesAsync();
            _logger.LogInformation("Awarded {Count} new badges", awarded);
        }
    }
}
