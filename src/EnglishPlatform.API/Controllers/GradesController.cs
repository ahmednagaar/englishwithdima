using EnglishPlatform.Application.DTOs.Content;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnglishPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public GradesController(IGradeService gradeService) => _gradeService = gradeService;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _gradeService.GetAllGradesAsync();
        return Ok(ApiResponse<List<GradeDto>>.Ok(result.Data!));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _gradeService.GetGradeByIdAsync(id);
        return result.Success ? Ok(ApiResponse<GradeDto>.Ok(result.Data!)) : NotFound(ApiResponse<GradeDto>.Fail(result.Errors));
    }

    [HttpGet("{id}/units")]
    public async Task<IActionResult> GetUnits(int id)
    {
        var result = await _gradeService.GetUnitsAsync(id);
        return Ok(ApiResponse<List<UnitDto>>.Ok(result.Data!));
    }

    [HttpGet("{gradeId}/units/{unitId}/lessons")]
    public async Task<IActionResult> GetLessons(int gradeId, int unitId)
    {
        var result = await _gradeService.GetLessonsAsync(unitId);
        return Ok(ApiResponse<List<LessonDto>>.Ok(result.Data!));
    }

    // ===== Admin Endpoints =====

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeDto dto)
    {
        var result = await _gradeService.CreateGradeAsync(dto);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, ApiResponse<GradeDto>.Ok(result.Data!))
                              : BadRequest(ApiResponse<GradeDto>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGrade(int id, [FromBody] CreateGradeDto dto)
    {
        var result = await _gradeService.UpdateGradeAsync(id, dto);
        return result.Success ? Ok(ApiResponse<GradeDto>.Ok(result.Data!)) : NotFound(ApiResponse<GradeDto>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost("units")]
    public async Task<IActionResult> CreateUnit([FromBody] CreateUnitDto dto)
    {
        var result = await _gradeService.CreateUnitAsync(dto);
        return result.Success ? Ok(ApiResponse<UnitDto>.Ok(result.Data!)) : BadRequest(ApiResponse<UnitDto>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("units/{id}")]
    public async Task<IActionResult> UpdateUnit(int id, [FromBody] CreateUnitDto dto)
    {
        var result = await _gradeService.UpdateUnitAsync(id, dto);
        return result.Success ? Ok(ApiResponse<UnitDto>.Ok(result.Data!)) : NotFound(ApiResponse<UnitDto>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost("lessons")]
    public async Task<IActionResult> CreateLesson([FromBody] CreateLessonDto dto)
    {
        var result = await _gradeService.CreateLessonAsync(dto);
        return result.Success ? Ok(ApiResponse<LessonDto>.Ok(result.Data!)) : BadRequest(ApiResponse<LessonDto>.Fail(result.Errors));
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("lessons/{id}")]
    public async Task<IActionResult> UpdateLesson(int id, [FromBody] CreateLessonDto dto)
    {
        var result = await _gradeService.UpdateLessonAsync(id, dto);
        return result.Success ? Ok(ApiResponse<LessonDto>.Ok(result.Data!)) : NotFound(ApiResponse<LessonDto>.Fail(result.Errors));
    }
}
