using EnglishPlatform.Application.DTOs.Games;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnglishPlatform.API.Controllers;

[ApiController]
[Route("api/games/matching")]
public class MatchingGamesController : ControllerBase
{
    private readonly IMatchingGameService _service;
    public MatchingGamesController(IMatchingGameService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GameFilterDto filter) =>
        Ok(ApiResponse<PagedList<MatchingGameDto>>.Ok((await _service.GetGamesAsync(filter)).Data!));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(ApiResponse<MatchingGameDto>.Ok(r.Data!)) : NotFound(ApiResponse<MatchingGameDto>.Fail(r.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMatchingGameDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var r = await _service.CreateAsync(dto, userId);
        return r.Success ? Ok(ApiResponse<MatchingGameDto>.Ok(r.Data!)) : BadRequest(ApiResponse<MatchingGameDto>.Fail(r.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateMatchingGameDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var r = await _service.UpdateAsync(id, dto, userId);
        return r.Success ? Ok(ApiResponse<MatchingGameDto>.Ok(r.Data!)) : NotFound(ApiResponse<MatchingGameDto>.Fail(r.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(ApiResponse<string>.Ok("Deleted")) : NotFound(ApiResponse<string>.Fail(r.Errors));
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        var r = await _service.StartSessionAsync(id, userId);
        return r.Success ? Ok(ApiResponse<MatchingGameStartDto>.Ok(r.Data!)) : BadRequest(ApiResponse<MatchingGameStartDto>.Fail(r.Errors));
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] MatchingSubmitDto dto)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        var r = await _service.SubmitSessionAsync(dto, userId);
        return r.Success ? Ok(ApiResponse<GameSessionResultDto>.Ok(r.Data!)) : BadRequest(ApiResponse<GameSessionResultDto>.Fail(r.Errors));
    }
}

[ApiController]
[Route("api/games/wheel")]
public class WheelGamesController : ControllerBase
{
    private readonly IWheelGameService _service;
    public WheelGamesController(IWheelGameService service) => _service = service;

    [HttpPost("{gradeId}/start")]
    public async Task<IActionResult> Start(int gradeId)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        var r = await _service.StartSessionAsync(gradeId, userId);
        return r.Success ? Ok(ApiResponse<WheelGameStartDto>.Ok(r.Data!)) : BadRequest(ApiResponse<WheelGameStartDto>.Fail(r.Errors));
    }

    [HttpPost("{sessionId}/spin")]
    public async Task<IActionResult> Spin(int sessionId)
    {
        var r = await _service.SpinAsync(sessionId);
        return r.Success ? Ok(ApiResponse<WheelSpinResultDto>.Ok(r.Data!)) : BadRequest(ApiResponse<WheelSpinResultDto>.Fail(r.Errors));
    }

    [HttpPost("answer")]
    public async Task<IActionResult> Answer([FromBody] WheelAnswerDto dto)
    {
        var r = await _service.AnswerAsync(dto);
        return r.Success ? Ok(ApiResponse<WheelAnswerResultDto>.Ok(r.Data!)) : BadRequest(ApiResponse<WheelAnswerResultDto>.Fail(r.Errors));
    }

    [HttpPost("{sessionId}/end")]
    public async Task<IActionResult> End(int sessionId)
    {
        var r = await _service.EndSessionAsync(sessionId);
        return r.Success ? Ok(ApiResponse<GameSessionResultDto>.Ok(r.Data!)) : BadRequest(ApiResponse<GameSessionResultDto>.Fail(r.Errors));
    }
}

[ApiController]
[Route("api/games/dragdrop")]
public class DragDropGamesController : ControllerBase
{
    private readonly IDragDropGameService _service;
    public DragDropGamesController(IDragDropGameService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GameFilterDto filter) =>
        Ok(ApiResponse<PagedList<DragDropGameDto>>.Ok((await _service.GetGamesAsync(filter)).Data!));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(ApiResponse<DragDropGameDto>.Ok(r.Data!)) : NotFound(ApiResponse<DragDropGameDto>.Fail(r.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDragDropGameDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var r = await _service.CreateAsync(dto, userId);
        return r.Success ? Ok(ApiResponse<DragDropGameDto>.Ok(r.Data!)) : BadRequest(ApiResponse<DragDropGameDto>.Fail(r.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(ApiResponse<string>.Ok("Deleted")) : NotFound(ApiResponse<string>.Fail(r.Errors));
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        var r = await _service.StartSessionAsync(id, userId);
        return r.Success ? Ok(ApiResponse<DragDropGameDto>.Ok(r.Data!)) : BadRequest(ApiResponse<DragDropGameDto>.Fail(r.Errors));
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] DragDropSubmitDto dto)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        var r = await _service.SubmitSessionAsync(dto, userId);
        return r.Success ? Ok(ApiResponse<GameSessionResultDto>.Ok(r.Data!)) : BadRequest(ApiResponse<GameSessionResultDto>.Fail(r.Errors));
    }
}

[ApiController]
[Route("api/games/flipcard")]
public class FlipCardGamesController : ControllerBase
{
    private readonly IFlipCardGameService _service;
    public FlipCardGamesController(IFlipCardGameService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GameFilterDto filter) =>
        Ok(ApiResponse<PagedList<FlipCardGameDto>>.Ok((await _service.GetGamesAsync(filter)).Data!));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var r = await _service.GetByIdAsync(id);
        return r.Success ? Ok(ApiResponse<FlipCardGameDto>.Ok(r.Data!)) : NotFound(ApiResponse<FlipCardGameDto>.Fail(r.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFlipCardGameDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var r = await _service.CreateAsync(dto, userId);
        return r.Success ? Ok(ApiResponse<FlipCardGameDto>.Ok(r.Data!)) : BadRequest(ApiResponse<FlipCardGameDto>.Fail(r.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.DeleteAsync(id);
        return r.Success ? Ok(ApiResponse<string>.Ok("Deleted")) : NotFound(ApiResponse<string>.Fail(r.Errors));
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> Start(int id)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        var r = await _service.StartSessionAsync(id, userId);
        return r.Success ? Ok(ApiResponse<FlipCardGameDto>.Ok(r.Data!)) : BadRequest(ApiResponse<FlipCardGameDto>.Fail(r.Errors));
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] FlipCardSubmitDto dto)
    {
        var userId = User.Identity?.IsAuthenticated == true ? User.FindFirstValue(ClaimTypes.NameIdentifier) : null;
        var r = await _service.SubmitSessionAsync(dto, userId);
        return r.Success ? Ok(ApiResponse<GameSessionResultDto>.Ok(r.Data!)) : BadRequest(ApiResponse<GameSessionResultDto>.Fail(r.Errors));
    }
}
