namespace EnglishPlatform.Application.DTOs.Content;

// ===== Grade DTOs =====
public class GradeDto
{
    public int Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Level { get; set; }
    public string SchoolType { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public int UnitsCount { get; set; }
}

public class CreateGradeDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Level { get; set; }
    public string SchoolType { get; set; } = "Primary";
    public int DisplayOrder { get; set; }
}

// ===== Unit DTOs =====
public class UnitDto
{
    public int Id { get; set; }
    public int GradeId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int UnitNumber { get; set; }
    public bool IsActive { get; set; }
    public int LessonsCount { get; set; }
}

public class CreateUnitDto
{
    public int GradeId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int UnitNumber { get; set; }
}

// ===== Lesson DTOs =====
public class LessonDto
{
    public int Id { get; set; }
    public int UnitId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int LessonNumber { get; set; }
    public bool IsActive { get; set; }
}

public class CreateLessonDto
{
    public int UnitId { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int LessonNumber { get; set; }
}
