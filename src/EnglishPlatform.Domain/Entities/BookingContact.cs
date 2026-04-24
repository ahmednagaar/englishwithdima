using EnglishPlatform.Domain.Enums;

namespace EnglishPlatform.Domain.Entities;

/// <summary>
/// Booking request for private lessons.
/// </summary>
public class BookingRequest : BaseEntity
{
    public int Id { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int GradeId { get; set; }
    public string? PreferredDates { get; set; }      // JSON array
    public string? Message { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    public string? AdminNotes { get; set; }

    // Navigation
    public virtual Grade Grade { get; set; } = null!;
}

/// <summary>
/// Contact form submission.
/// </summary>
public class ContactMessage : BaseEntity
{
    public int Id { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? SenderPhone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; } = false;
}
