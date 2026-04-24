using EnglishPlatform.Application.DTOs.Games;
using EnglishPlatform.Shared;

namespace EnglishPlatform.Application.Interfaces;

public interface IMatchingGameService
{
    Task<Result<PagedList<MatchingGameDto>>> GetGamesAsync(GameFilterDto filter);
    Task<Result<MatchingGameDto>> GetByIdAsync(int id);
    Task<Result<MatchingGameDto>> CreateAsync(CreateMatchingGameDto dto, string userId);
    Task<Result<MatchingGameDto>> UpdateAsync(int id, CreateMatchingGameDto dto, string userId);
    Task<Result> DeleteAsync(int id);
    Task<Result<MatchingGameStartDto>> StartSessionAsync(int gameId, string? userId);
    Task<Result<GameSessionResultDto>> SubmitSessionAsync(MatchingSubmitDto dto, string? userId);
}

public interface IWheelGameService
{
    Task<Result<WheelGameStartDto>> StartSessionAsync(int gradeId, string? userId);
    Task<Result<WheelSpinResultDto>> SpinAsync(int sessionId);
    Task<Result<WheelAnswerResultDto>> AnswerAsync(WheelAnswerDto dto);
    Task<Result<GameSessionResultDto>> EndSessionAsync(int sessionId);
}

public interface IDragDropGameService
{
    Task<Result<PagedList<DragDropGameDto>>> GetGamesAsync(GameFilterDto filter);
    Task<Result<DragDropGameDto>> GetByIdAsync(int id);
    Task<Result<DragDropGameDto>> CreateAsync(CreateDragDropGameDto dto, string userId);
    Task<Result<DragDropGameDto>> UpdateAsync(int id, CreateDragDropGameDto dto, string userId);
    Task<Result> DeleteAsync(int id);
    Task<Result<DragDropGameDto>> StartSessionAsync(int gameId, string? userId);
    Task<Result<GameSessionResultDto>> SubmitSessionAsync(DragDropSubmitDto dto, string? userId);
}

public interface IFlipCardGameService
{
    Task<Result<PagedList<FlipCardGameDto>>> GetGamesAsync(GameFilterDto filter);
    Task<Result<FlipCardGameDto>> GetByIdAsync(int id);
    Task<Result<FlipCardGameDto>> CreateAsync(CreateFlipCardGameDto dto, string userId);
    Task<Result<FlipCardGameDto>> UpdateAsync(int id, CreateFlipCardGameDto dto, string userId);
    Task<Result> DeleteAsync(int id);
    Task<Result<FlipCardGameDto>> StartSessionAsync(int gameId, string? userId);
    Task<Result<GameSessionResultDto>> SubmitSessionAsync(FlipCardSubmitDto dto, string? userId);
}
