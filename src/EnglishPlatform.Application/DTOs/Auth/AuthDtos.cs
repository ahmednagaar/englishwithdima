namespace EnglishPlatform.Application.DTOs.Auth;

public class RegisterDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Role { get; set; } = "Student"; // Student or Parent
    public int? GradeId { get; set; }              // Required if Student
    public string PreferredLanguage { get; set; } = "ar";
}

public class LoginDto
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class StudentPinLoginDto
{
    public string StudentCode { get; set; } = string.Empty;
    public string Pin { get; set; } = string.Empty;
}

public class FacebookLoginDto
{
    public string AccessToken { get; set; } = string.Empty;
}

public class GuestCreateDto
{
    public string DisplayName { get; set; } = string.Empty;
    public int GradeId { get; set; }
}

public class RefreshTokenDto
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? GradeId { get; set; }
    public string? AvatarUrl { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
}

public class GuestSessionDto
{
    public Guid SessionId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public int GradeId { get; set; }
    public string SessionToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

public class UserProfileDto
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Role { get; set; } = string.Empty;
    public int? GradeId { get; set; }
    public string? GradeName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? StudentCode { get; set; }
    public string PreferredLanguage { get; set; } = "ar";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
}

public class UpdateProfileDto
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AvatarUrl { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? Pin { get; set; }
}
