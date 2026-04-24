using EnglishPlatform.Application.DTOs.Tests;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnglishPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestsController : ControllerBase
{
    private readonly ITestService _testService;

    public TestsController(ITestService testService) => _testService = testService;

    // ===== Public / Student Endpoints =====

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TestFilterDto filter)
    {
        var result = await _testService.GetTestsAsync(filter);
        return Ok(ApiResponse<PagedList<TestDto>>.Ok(result.Data!, result.Data!.Meta));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _testService.GetTestWithQuestionsAsync(id);
        return result.Success ? Ok(ApiResponse<TestDetailDto>.Ok(result.Data!)) : NotFound(ApiResponse<TestDetailDto>.Fail(result.Errors));
    }

    // ===== Test-Taking Endpoints =====

    [Authorize]
    [HttpPost("{id}/start")]
    public async Task<IActionResult> StartAttempt(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _testService.StartAttemptAsync(id, userId);
        return result.Success ? Ok(ApiResponse<AttemptStartDto>.Ok(result.Data!)) : BadRequest(ApiResponse<AttemptStartDto>.Fail(result.Errors));
    }

    [Authorize]
    [HttpPost("{id}/submit")]
    public async Task<IActionResult> SubmitAttempt(int id, [FromBody] SubmitAttemptDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        dto.TestId = id;
        var result = await _testService.SubmitAttemptAsync(dto, userId);
        return result.Success ? Ok(ApiResponse<AttemptResultDto>.Ok(result.Data!)) : BadRequest(ApiResponse<AttemptResultDto>.Fail(result.Errors));
    }

    [Authorize]
    [HttpGet("{id}/results")]
    public async Task<IActionResult> GetUserAttempts(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _testService.GetUserAttemptsAsync(id, userId);
        return Ok(ApiResponse<List<AttemptSummaryDto>>.Ok(result.Data!));
    }

    [Authorize]
    [HttpGet("attempts/{attemptId}")]
    public async Task<IActionResult> GetAttemptDetail(int attemptId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _testService.GetAttemptDetailAsync(attemptId, userId);
        return result.Success ? Ok(ApiResponse<AttemptResultDto>.Ok(result.Data!)) : NotFound(ApiResponse<AttemptResultDto>.Fail(result.Errors));
    }

    // ===== Admin Endpoints =====

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _testService.CreateTestAsync(dto, userId);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<TestDto>.Ok(result.Data!))
                              : BadRequest(ApiResponse<TestDto>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        dto.Id = id;
        var result = await _testService.UpdateTestAsync(id, dto, userId);
        return result.Success ? Ok(ApiResponse<TestDto>.Ok(result.Data!)) : NotFound(ApiResponse<TestDto>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _testService.DeleteTestAsync(id);
        return result.Success ? Ok(ApiResponse<string>.Ok("Deleted")) : NotFound(ApiResponse<string>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpPost("{id}/publish")]
    public async Task<IActionResult> Publish(int id, [FromQuery] bool publish = true)
    {
        var result = await _testService.PublishTestAsync(id, publish);
        return result.Success ? Ok(ApiResponse<string>.Ok(result.Message!)) : NotFound(ApiResponse<string>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpPost("{id}/questions")]
    public async Task<IActionResult> AddQuestions(int id, [FromBody] AddQuestionsToTestDto dto)
    {
        var result = await _testService.AddQuestionsToTestAsync(id, dto.QuestionIds);
        return result.Success ? Ok(ApiResponse<string>.Ok("Questions added")) : BadRequest(ApiResponse<string>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpDelete("{id}/questions")]
    public async Task<IActionResult> RemoveQuestions(int id, [FromBody] AddQuestionsToTestDto dto)
    {
        var result = await _testService.RemoveQuestionsFromTestAsync(id, dto.QuestionIds);
        return result.Success ? Ok(ApiResponse<string>.Ok("Questions removed")) : BadRequest(ApiResponse<string>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin,Teacher")]
    [HttpPut("{id}/reorder")]
    public async Task<IActionResult> ReorderQuestions(int id, [FromBody] ReorderQuestionsDto dto)
    {
        var result = await _testService.ReorderQuestionsAsync(id, dto);
        return result.Success ? Ok(ApiResponse<string>.Ok("Reordered")) : BadRequest(ApiResponse<string>.Fail(result.Errors));
    }
}
