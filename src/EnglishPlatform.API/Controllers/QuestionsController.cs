using EnglishPlatform.Application.DTOs.Questions;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnglishPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,SuperAdmin,Teacher")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionsController(IQuestionService questionService) => _questionService = questionService;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QuestionFilterDto filter)
    {
        var result = await _questionService.GetQuestionsAsync(filter);
        return Ok(ApiResponse<PagedList<QuestionDto>>.Ok(result.Data!, result.Data!.Meta));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _questionService.GetQuestionByIdAsync(id);
        return result.Success ? Ok(ApiResponse<QuestionDto>.Ok(result.Data!)) : NotFound(ApiResponse<QuestionDto>.Fail(result.Errors));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _questionService.CreateQuestionAsync(dto, userId);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<QuestionDto>.Ok(result.Data!))
                              : BadRequest(ApiResponse<QuestionDto>.Fail(result.Errors));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuestionDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        dto.Id = id;
        var result = await _questionService.UpdateQuestionAsync(id, dto, userId);
        return result.Success ? Ok(ApiResponse<QuestionDto>.Ok(result.Data!)) : NotFound(ApiResponse<QuestionDto>.Fail(result.Errors));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _questionService.DeleteQuestionAsync(id);
        return result.Success ? Ok(ApiResponse<string>.Ok("Deleted")) : NotFound(ApiResponse<string>.Fail(result.Errors));
    }

    [HttpPost("bulk-import")]
    public async Task<IActionResult> BulkImport([FromBody] List<CreateQuestionDto> questions)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _questionService.BulkImportAsync(questions, userId);
        return Ok(ApiResponse<List<QuestionDto>>.Ok(result.Data!, result.Message));
    }
}
