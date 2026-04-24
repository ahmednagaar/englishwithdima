using AutoMapper;
using EnglishPlatform.Application.DTOs.Questions;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.UnitOfWork;
using EnglishPlatform.Shared;
using Microsoft.EntityFrameworkCore;

namespace EnglishPlatform.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QuestionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PagedList<QuestionDto>>> GetQuestionsAsync(QuestionFilterDto filter)
    {
        var query = _unitOfWork.Questions.Query()
            .Include(q => q.QuestionOptions)
            .Include(q => q.MatchingPairs)
            .Include(q => q.SubQuestions)
            .AsQueryable();

        // Apply filters
        if (filter.GradeId.HasValue)
            query = query.Where(q => q.GradeId == filter.GradeId.Value);
        if (filter.QuestionType.HasValue)
            query = query.Where(q => q.QuestionType == filter.QuestionType.Value);
        if (filter.DifficultyLevel.HasValue)
            query = query.Where(q => q.DifficultyLevel == filter.DifficultyLevel.Value);
        if (filter.SkillCategory.HasValue)
            query = query.Where(q => q.SkillCategory == filter.SkillCategory.Value);
        if (filter.ContentTopic.HasValue)
            query = query.Where(q => q.ContentTopic == filter.ContentTopic.Value);
        if (filter.TestType.HasValue)
            query = query.Where(q => q.TestType == filter.TestType.Value);
        if (!string.IsNullOrEmpty(filter.SearchTerm))
            query = query.Where(q => q.QuestionText.Contains(filter.SearchTerm));

        query = query.OrderByDescending(q => q.CreatedAt);

        var pagedList = await query
            .Select(q => _mapper.Map<QuestionDto>(q))
            .ToPagedListAsync(filter.PageNumber, filter.PageSize);

        return Result<PagedList<QuestionDto>>.Ok(pagedList);
    }

    public async Task<Result<QuestionDto>> GetQuestionByIdAsync(int id)
    {
        var question = await _unitOfWork.Questions.Query()
            .Include(q => q.QuestionOptions)
            .Include(q => q.MatchingPairs)
            .Include(q => q.SubQuestions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return Result<QuestionDto>.Fail("السؤال غير موجود / Question not found");

        return Result<QuestionDto>.Ok(_mapper.Map<QuestionDto>(question));
    }

    public async Task<Result<QuestionDto>> CreateQuestionAsync(CreateQuestionDto dto, string userId)
    {
        var question = _mapper.Map<Question>(dto);
        question.CreatedBy = userId;
        question.CreatedAt = DateTime.UtcNow;

        await _unitOfWork.Questions.AddAsync(question);

        // Add options for MCQ
        if (dto.Options?.Any() == true)
        {
            foreach (var optDto in dto.Options)
            {
                var option = _mapper.Map<QuestionOption>(optDto);
                option.Question = question;
                await _unitOfWork.QuestionOptions.AddAsync(option);
            }
        }

        // Add matching pairs
        if (dto.MatchingPairs?.Any() == true)
        {
            foreach (var pairDto in dto.MatchingPairs)
            {
                var pair = _mapper.Map<MatchingPair>(pairDto);
                pair.Question = question;
                await _unitOfWork.QuestionMatchingPairs.AddAsync(pair);
            }
        }

        // Add sub-questions for passage type
        if (dto.SubQuestions?.Any() == true)
        {
            foreach (var subDto in dto.SubQuestions)
            {
                var sub = _mapper.Map<SubQuestion>(subDto);
                sub.Question = question;
                await _unitOfWork.SubQuestions.AddAsync(sub);
            }
        }

        await _unitOfWork.SaveChangesAsync();

        return Result<QuestionDto>.Ok(_mapper.Map<QuestionDto>(question));
    }

    public async Task<Result<QuestionDto>> UpdateQuestionAsync(int id, UpdateQuestionDto dto, string userId)
    {
        var question = await _unitOfWork.Questions.Query()
            .Include(q => q.QuestionOptions)
            .Include(q => q.MatchingPairs)
            .Include(q => q.SubQuestions)
            .FirstOrDefaultAsync(q => q.Id == id);

        if (question == null)
            return Result<QuestionDto>.Fail("السؤال غير موجود / Question not found");

        // Update base fields
        question.QuestionText = dto.QuestionText;
        question.InstructionAr = dto.InstructionAr;
        question.QuestionType = dto.QuestionType;
        question.DifficultyLevel = dto.DifficultyLevel;
        question.SkillCategory = dto.SkillCategory;
        question.ContentTopic = dto.ContentTopic;
        question.GradeId = dto.GradeId;
        question.TestType = dto.TestType;
        question.ImageUrl = dto.ImageUrl;
        question.AudioUrl = dto.AudioUrl;
        question.VideoUrl = dto.VideoUrl;
        question.PassageText = dto.PassageText;
        question.CorrectAnswer = dto.CorrectAnswer;
        question.Explanation = dto.Explanation;
        question.HintText = dto.HintText;
        question.Tags = dto.Tags;
        question.Points = dto.Points;
        question.EstimatedTimeMinutes = dto.EstimatedTimeMinutes;
        question.UpdatedBy = userId;
        question.UpdatedAt = DateTime.UtcNow;

        // Replace options
        if (dto.Options != null)
        {
            foreach (var old in question.QuestionOptions.ToList())
                _unitOfWork.QuestionOptions.Delete(old);

            foreach (var optDto in dto.Options)
            {
                var option = _mapper.Map<QuestionOption>(optDto);
                option.QuestionId = id;
                await _unitOfWork.QuestionOptions.AddAsync(option);
            }
        }

        // Replace matching pairs
        if (dto.MatchingPairs != null)
        {
            foreach (var old in question.MatchingPairs.ToList())
                _unitOfWork.QuestionMatchingPairs.Delete(old);

            foreach (var pairDto in dto.MatchingPairs)
            {
                var pair = _mapper.Map<MatchingPair>(pairDto);
                pair.QuestionId = id;
                await _unitOfWork.QuestionMatchingPairs.AddAsync(pair);
            }
        }

        // Replace sub-questions
        if (dto.SubQuestions != null)
        {
            foreach (var old in question.SubQuestions.ToList())
                _unitOfWork.SubQuestions.Delete(old);

            foreach (var subDto in dto.SubQuestions)
            {
                var sub = _mapper.Map<SubQuestion>(subDto);
                sub.QuestionId = id;
                await _unitOfWork.SubQuestions.AddAsync(sub);
            }
        }

        _unitOfWork.Questions.Update(question);
        await _unitOfWork.SaveChangesAsync();

        return Result<QuestionDto>.Ok(_mapper.Map<QuestionDto>(question));
    }

    public async Task<Result> DeleteQuestionAsync(int id)
    {
        var question = await _unitOfWork.Questions.GetByIdAsync(id);
        if (question == null)
            return Result.Fail("السؤال غير موجود / Question not found");

        _unitOfWork.Questions.Delete(question); // Soft delete via SaveChanges interceptor
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok("تم حذف السؤال بنجاح / Question deleted successfully");
    }

    public async Task<Result<List<QuestionDto>>> BulkImportAsync(List<CreateQuestionDto> questions, string userId)
    {
        var created = new List<QuestionDto>();

        foreach (var dto in questions)
        {
            var result = await CreateQuestionAsync(dto, userId);
            if (result.Success && result.Data != null)
                created.Add(result.Data);
        }

        return Result<List<QuestionDto>>.Ok(created, $"تم استيراد {created.Count} سؤال بنجاح / {created.Count} questions imported");
    }
}
