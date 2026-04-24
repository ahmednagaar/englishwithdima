using AutoMapper;
using EnglishPlatform.Application.DTOs.Content;
using EnglishPlatform.Application.DTOs.Questions;
using EnglishPlatform.Application.DTOs.Tests;
using EnglishPlatform.Domain.Entities;

namespace EnglishPlatform.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ===== Content Hierarchy =====
        CreateMap<Grade, GradeDto>()
            .ForMember(d => d.UnitsCount, o => o.MapFrom(s => s.Units.Count));
        CreateMap<CreateGradeDto, Grade>();

        CreateMap<Unit, UnitDto>()
            .ForMember(d => d.LessonsCount, o => o.MapFrom(s => s.Lessons.Count));
        CreateMap<CreateUnitDto, Unit>();

        CreateMap<Lesson, LessonDto>();
        CreateMap<CreateLessonDto, Lesson>();

        // ===== Questions =====
        CreateMap<Question, QuestionDto>()
            .ForMember(d => d.Options, o => o.MapFrom(s => s.QuestionOptions))
            .ForMember(d => d.MatchingPairs, o => o.MapFrom(s => s.MatchingPairs))
            .ForMember(d => d.SubQuestions, o => o.MapFrom(s => s.SubQuestions));

        CreateMap<CreateQuestionDto, Question>();
        CreateMap<UpdateQuestionDto, Question>();

        CreateMap<QuestionOption, QuestionOptionDto>();
        CreateMap<CreateQuestionOptionDto, QuestionOption>();

        CreateMap<MatchingPair, MatchingPairDto>();
        CreateMap<CreateMatchingPairDto, MatchingPair>();

        CreateMap<SubQuestion, SubQuestionDto>();
        CreateMap<CreateSubQuestionDto, SubQuestion>();

        // ===== Tests =====
        CreateMap<Test, TestDto>()
            .ForMember(d => d.GradeName, o => o.MapFrom(s => s.Grade != null ? s.Grade.NameEn : null))
            .ForMember(d => d.UnitName, o => o.MapFrom(s => s.Unit != null ? s.Unit.NameEn : null))
            .ForMember(d => d.LessonName, o => o.MapFrom(s => s.Lesson != null ? s.Lesson.NameEn : null))
            .ForMember(d => d.QuestionCount, o => o.MapFrom(s => s.TestQuestions.Count));

        CreateMap<Test, TestDetailDto>()
            .ForMember(d => d.GradeName, o => o.MapFrom(s => s.Grade != null ? s.Grade.NameEn : null))
            .ForMember(d => d.UnitName, o => o.MapFrom(s => s.Unit != null ? s.Unit.NameEn : null))
            .ForMember(d => d.LessonName, o => o.MapFrom(s => s.Lesson != null ? s.Lesson.NameEn : null))
            .ForMember(d => d.QuestionCount, o => o.MapFrom(s => s.TestQuestions.Count))
            .ForMember(d => d.Questions, o => o.MapFrom(s => s.TestQuestions
                .OrderBy(tq => tq.OrderIndex)
                .Select(tq => tq.Question)));

        CreateMap<CreateTestDto, Test>();
        CreateMap<UpdateTestDto, Test>();

        // ===== Test Results =====
        CreateMap<TestAttempt, AttemptSummaryDto>()
            .ForMember(d => d.AttemptId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.TestTitle, o => o.MapFrom(s => s.Test != null ? s.Test.TitleEn : ""))
            .ForMember(d => d.SubmittedAt, o => o.MapFrom(s => s.SubmittedAt ?? s.StartedAt));
    }
}
