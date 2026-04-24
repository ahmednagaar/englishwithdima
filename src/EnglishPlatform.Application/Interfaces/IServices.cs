using EnglishPlatform.Application.DTOs.Auth;
using EnglishPlatform.Application.DTOs.Content;
using EnglishPlatform.Application.DTOs.Questions;
using EnglishPlatform.Application.DTOs.Tests;
using EnglishPlatform.Shared;

namespace EnglishPlatform.Application.Interfaces;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<AuthResponseDto>> StudentPinLoginAsync(StudentPinLoginDto dto);
    Task<Result<AuthResponseDto>> FacebookLoginAsync(string accessToken);
    Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
    Task<Result<GuestSessionDto>> CreateGuestSessionAsync(GuestCreateDto dto);
    Task<Result<UserProfileDto>> GetProfileAsync(string userId);
    Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    Task<Result> LogoutAsync(string userId, string refreshToken);
}

public interface IGradeService
{
    Task<Result<List<GradeDto>>> GetAllGradesAsync();
    Task<Result<GradeDto>> GetGradeByIdAsync(int id);
    Task<Result<List<UnitDto>>> GetUnitsAsync(int gradeId);
    Task<Result<List<LessonDto>>> GetLessonsAsync(int unitId);
    Task<Result<GradeDto>> CreateGradeAsync(CreateGradeDto dto);
    Task<Result<GradeDto>> UpdateGradeAsync(int id, CreateGradeDto dto);
    Task<Result<UnitDto>> CreateUnitAsync(CreateUnitDto dto);
    Task<Result<UnitDto>> UpdateUnitAsync(int id, CreateUnitDto dto);
    Task<Result<LessonDto>> CreateLessonAsync(CreateLessonDto dto);
    Task<Result<LessonDto>> UpdateLessonAsync(int id, CreateLessonDto dto);
}

public interface IQuestionService
{
    Task<Result<PagedList<QuestionDto>>> GetQuestionsAsync(QuestionFilterDto filter);
    Task<Result<QuestionDto>> GetQuestionByIdAsync(int id);
    Task<Result<QuestionDto>> CreateQuestionAsync(CreateQuestionDto dto, string userId);
    Task<Result<QuestionDto>> UpdateQuestionAsync(int id, UpdateQuestionDto dto, string userId);
    Task<Result> DeleteQuestionAsync(int id);
    Task<Result<List<QuestionDto>>> BulkImportAsync(List<CreateQuestionDto> questions, string userId);
}

public interface ITestService
{
    Task<Result<PagedList<TestDto>>> GetTestsAsync(TestFilterDto filter);
    Task<Result<TestDetailDto>> GetTestWithQuestionsAsync(int testId);
    Task<Result<TestDto>> CreateTestAsync(CreateTestDto dto, string userId);
    Task<Result<TestDto>> UpdateTestAsync(int id, UpdateTestDto dto, string userId);
    Task<Result> DeleteTestAsync(int id);
    Task<Result> PublishTestAsync(int id, bool publish);
    Task<Result> AddQuestionsToTestAsync(int testId, List<int> questionIds);
    Task<Result> RemoveQuestionsFromTestAsync(int testId, List<int> questionIds);
    Task<Result> ReorderQuestionsAsync(int testId, ReorderQuestionsDto dto);

    // Test-taking
    Task<Result<AttemptStartDto>> StartAttemptAsync(int testId, string userId);
    Task<Result<AttemptResultDto>> SubmitAttemptAsync(SubmitAttemptDto dto, string userId);
    Task<Result<List<AttemptSummaryDto>>> GetUserAttemptsAsync(int testId, string userId);
    Task<Result<AttemptResultDto>> GetAttemptDetailAsync(int attemptId, string userId);
    Task<bool> CanUserRetryAsync(int testId, string userId);
}
