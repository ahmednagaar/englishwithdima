using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.Data;
using EnglishPlatform.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Infrastructure.Repositories.Implementations;

public class TestRepository : GenericRepository<Test>, ITestRepository
{
    public TestRepository(AppDbContext context) : base(context) { }

    public async Task<Test?> GetWithQuestionsAsync(int testId)
    {
        return await _dbSet
            .Include(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                    .ThenInclude(q => q.QuestionOptions)
            .Include(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                    .ThenInclude(q => q.MatchingPairs)
            .Include(t => t.TestQuestions)
                .ThenInclude(tq => tq.Question)
                    .ThenInclude(q => q.SubQuestions)
            .Include(t => t.Grade)
            .Include(t => t.Unit)
            .Include(t => t.Lesson)
            .FirstOrDefaultAsync(t => t.Id == testId);
    }

    public async Task<List<Test>> GetByGradeAsync(int gradeId, string? testType = null)
    {
        var query = _dbSet
            .Include(t => t.Grade)
            .Where(t => t.GradeId == gradeId && t.IsPublished);

        if (!string.IsNullOrEmpty(testType) && Enum.TryParse<Domain.Enums.TestType>(testType, out var type))
        {
            query = query.Where(t => t.TestType == type);
        }

        return await query.OrderBy(t => t.CreatedAt).ToListAsync();
    }

    public async Task<int> CountAttemptsAsync(int testId, string userId)
    {
        return await _context.TestAttempts
            .CountAsync(a => a.TestId == testId && a.UserId == userId && a.Status == Domain.Enums.AttemptStatus.Completed);
    }
}

public class LeaderboardRepository : ILeaderboardRepository
{
    private readonly AppDbContext _context;

    public LeaderboardRepository(AppDbContext context) => _context = context;

    public async Task<List<LeaderboardEntry>> GetGradeTopAsync(int gradeId, int count = 50)
    {
        return await _context.LeaderboardEntries
            .Where(l => l.GradeId == gradeId)
            .OrderByDescending(l => l.TotalPoints)
            .Take(count)
            .ToListAsync();
    }

    public async Task UpsertUserScoreAsync(string userId, int gradeId, int points, string displayName, string? avatarUrl)
    {
        var entry = await _context.LeaderboardEntries
            .FirstOrDefaultAsync(l => l.UserId == userId && l.GradeId == gradeId);

        if (entry == null)
        {
            entry = new LeaderboardEntry
            {
                UserId = userId,
                GradeId = gradeId,
                TotalPoints = points,
                DisplayName = displayName,
                AvatarUrl = avatarUrl
            };
            await _context.LeaderboardEntries.AddAsync(entry);
        }
        else
        {
            entry.TotalPoints = points;
            entry.DisplayName = displayName;
            entry.AvatarUrl = avatarUrl;
            entry.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task RefreshRanksAsync(int gradeId)
    {
        var entries = await _context.LeaderboardEntries
            .Where(l => l.GradeId == gradeId)
            .OrderByDescending(l => l.TotalPoints)
            .ToListAsync();

        for (int i = 0; i < entries.Count; i++)
        {
            entries[i].Rank = i + 1;
            entries[i].UpdatedAt = DateTime.UtcNow;
        }
    }
}

public class StudentProgressRepository : IStudentProgressRepository
{
    private readonly AppDbContext _context;

    public StudentProgressRepository(AppDbContext context) => _context = context;

    public async Task<StudentProgress?> GetByUserAndGradeAsync(string userId, int gradeId)
    {
        return await _context.StudentProgress
            .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.GradeId == gradeId);
    }

    public async Task UpsertProgressAsync(StudentProgress progress)
    {
        var existing = await _context.StudentProgress
            .FirstOrDefaultAsync(sp => sp.UserId == progress.UserId && sp.GradeId == progress.GradeId);

        if (existing == null)
        {
            await _context.StudentProgress.AddAsync(progress);
        }
        else
        {
            existing.TotalPoints = progress.TotalPoints;
            existing.TotalTestsTaken = progress.TotalTestsTaken;
            existing.TotalGamesPlayed = progress.TotalGamesPlayed;
            existing.AverageScore = progress.AverageScore;
            existing.CurrentStreak = progress.CurrentStreak;
            existing.LongestStreak = progress.LongestStreak;
            existing.LastActivityAt = progress.LastActivityAt;
            existing.UpdatedAt = DateTime.UtcNow;
        }
    }
}
