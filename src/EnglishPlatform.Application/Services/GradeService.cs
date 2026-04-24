using AutoMapper;
using EnglishPlatform.Application.DTOs.Content;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.UnitOfWork;
using EnglishPlatform.Shared;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Application.Services;

public class GradeService : IGradeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GradeService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<GradeDto>>> GetAllGradesAsync()
    {
        var grades = await _unitOfWork.Grades.Query()
            .Include(g => g.Units)
            .Where(g => g.IsActive)
            .OrderBy(g => g.DisplayOrder)
            .ToListAsync();

        return Result<List<GradeDto>>.Ok(_mapper.Map<List<GradeDto>>(grades));
    }

    public async Task<Result<GradeDto>> GetGradeByIdAsync(int id)
    {
        var grade = await _unitOfWork.Grades.Query()
            .Include(g => g.Units)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (grade == null)
            return Result<GradeDto>.Fail("الصف غير موجود / Grade not found");

        return Result<GradeDto>.Ok(_mapper.Map<GradeDto>(grade));
    }

    public async Task<Result<List<UnitDto>>> GetUnitsAsync(int gradeId)
    {
        var units = await _unitOfWork.Units.Query()
            .Include(u => u.Lessons)
            .Where(u => u.GradeId == gradeId && u.IsActive)
            .OrderBy(u => u.UnitNumber)
            .ToListAsync();

        return Result<List<UnitDto>>.Ok(_mapper.Map<List<UnitDto>>(units));
    }

    public async Task<Result<List<LessonDto>>> GetLessonsAsync(int unitId)
    {
        var lessons = await _unitOfWork.Lessons.Query()
            .Where(l => l.UnitId == unitId && l.IsActive)
            .OrderBy(l => l.LessonNumber)
            .ToListAsync();

        return Result<List<LessonDto>>.Ok(_mapper.Map<List<LessonDto>>(lessons));
    }

    public async Task<Result<GradeDto>> CreateGradeAsync(CreateGradeDto dto)
    {
        var grade = _mapper.Map<Grade>(dto);
        grade.IsActive = true;
        await _unitOfWork.Grades.AddAsync(grade);
        await _unitOfWork.SaveChangesAsync();
        return Result<GradeDto>.Ok(_mapper.Map<GradeDto>(grade));
    }

    public async Task<Result<GradeDto>> UpdateGradeAsync(int id, CreateGradeDto dto)
    {
        var grade = await _unitOfWork.Grades.GetByIdAsync(id);
        if (grade == null) return Result<GradeDto>.Fail("Grade not found");

        grade.NameAr = dto.NameAr;
        grade.NameEn = dto.NameEn;
        grade.Level = dto.Level;
        grade.SchoolType = dto.SchoolType;
        grade.DisplayOrder = dto.DisplayOrder;
        _unitOfWork.Grades.Update(grade);
        await _unitOfWork.SaveChangesAsync();
        return Result<GradeDto>.Ok(_mapper.Map<GradeDto>(grade));
    }

    public async Task<Result<UnitDto>> CreateUnitAsync(CreateUnitDto dto)
    {
        var unit = _mapper.Map<Unit>(dto);
        unit.IsActive = true;
        await _unitOfWork.Units.AddAsync(unit);
        await _unitOfWork.SaveChangesAsync();
        return Result<UnitDto>.Ok(_mapper.Map<UnitDto>(unit));
    }

    public async Task<Result<UnitDto>> UpdateUnitAsync(int id, CreateUnitDto dto)
    {
        var unit = await _unitOfWork.Units.GetByIdAsync(id);
        if (unit == null) return Result<UnitDto>.Fail("Unit not found");

        unit.NameAr = dto.NameAr;
        unit.NameEn = dto.NameEn;
        unit.UnitNumber = dto.UnitNumber;
        unit.GradeId = dto.GradeId;
        _unitOfWork.Units.Update(unit);
        await _unitOfWork.SaveChangesAsync();
        return Result<UnitDto>.Ok(_mapper.Map<UnitDto>(unit));
    }

    public async Task<Result<LessonDto>> CreateLessonAsync(CreateLessonDto dto)
    {
        var lesson = _mapper.Map<Lesson>(dto);
        lesson.IsActive = true;
        await _unitOfWork.Lessons.AddAsync(lesson);
        await _unitOfWork.SaveChangesAsync();
        return Result<LessonDto>.Ok(_mapper.Map<LessonDto>(lesson));
    }

    public async Task<Result<LessonDto>> UpdateLessonAsync(int id, CreateLessonDto dto)
    {
        var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);
        if (lesson == null) return Result<LessonDto>.Fail("Lesson not found");

        lesson.NameAr = dto.NameAr;
        lesson.NameEn = dto.NameEn;
        lesson.LessonNumber = dto.LessonNumber;
        lesson.UnitId = dto.UnitId;
        _unitOfWork.Lessons.Update(lesson);
        await _unitOfWork.SaveChangesAsync();
        return Result<LessonDto>.Ok(_mapper.Map<LessonDto>(lesson));
    }
}
