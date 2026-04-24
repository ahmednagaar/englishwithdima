using EnglishPlatform.Domain.Entities;

namespace EnglishPlatform.Infrastructure.Repositories.Interfaces;

public interface ITestRepository : IGenericRepository<Test>
{
    Task<Test?> GetWithQuestionsAsync(int testId);
    Task<List<Test>> GetByGradeAsync(int gradeId, string? testType = null);
    Task<int> CountAttemptsAsync(int testId, string userId);
}

public interface ILeaderboardRepository
{
    Task<List<LeaderboardEntry>> GetGradeTopAsync(int gradeId, int count = 50);
    Task UpsertUserScoreAsync(string userId, int gradeId, int points, string displayName, string? avatarUrl);
    Task RefreshRanksAsync(int gradeId);
}

public interface IStudentProgressRepository
{
    Task<StudentProgress?> GetByUserAndGradeAsync(string userId, int gradeId);
    Task UpsertProgressAsync(StudentProgress progress);
}
