using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.UnitOfWork;
using EnglishPlatform.Shared;
using Microsoft.AspNetCore.Mvc;

namespace EnglishPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ContactController(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    [HttpPost("message")]
    public async Task<IActionResult> SubmitContactMessage([FromBody] ContactMessageRequestDto dto)
    {
        var message = new ContactMessage
        {
            SenderName = dto.SenderName,
            SenderEmail = dto.SenderEmail,
            SenderPhone = dto.SenderPhone,
            Subject = dto.Subject,
            Message = dto.Message,
            SubmittedAt = DateTime.UtcNow
        };

        await _unitOfWork.ContactMessages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("تم إرسال رسالتك بنجاح / Message sent successfully"));
    }

    [HttpPost("booking")]
    public async Task<IActionResult> SubmitBooking([FromBody] BookingSubmitDto dto)
    {
        var booking = new BookingRequest
        {
            ParentName = dto.ParentName,
            StudentName = dto.StudentName,
            Email = dto.Email,
            Phone = dto.Phone,
            GradeId = dto.GradeId,
            PreferredDates = dto.PreferredDates,
            Message = dto.Message,
            SubmittedAt = DateTime.UtcNow
        };

        await _unitOfWork.BookingRequests.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        return Ok(ApiResponse<string>.Ok("تم إرسال طلب الحجز بنجاح / Booking request submitted"));
    }
}

// ===== Request DTOs =====
public class ContactMessageRequestDto
{
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    public string? SenderPhone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class BookingSubmitDto
{
    public string ParentName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int GradeId { get; set; }
    public string? PreferredDates { get; set; }
    public string? Message { get; set; }
}
