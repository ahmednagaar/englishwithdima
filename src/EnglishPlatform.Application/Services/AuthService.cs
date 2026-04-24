using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EnglishPlatform.Application.DTOs.Auth;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Domain.Entities;
using EnglishPlatform.Infrastructure.UnitOfWork;
using EnglishPlatform.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace EnglishPlatform.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;

    public AuthService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IConfiguration config)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _config = config;
    }

    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _userManager.FindByNameAsync(dto.UserName);
        if (existingUser != null)
            return Result<AuthResponseDto>.Fail("اسم المستخدم مُستخدم بالفعل / Username already taken");

        if (!string.IsNullOrEmpty(dto.Email))
        {
            var emailExists = await _userManager.FindByEmailAsync(dto.Email);
            if (emailExists != null)
                return Result<AuthResponseDto>.Fail("البريد الإلكتروني مُستخدم بالفعل / Email already taken");
        }

        if (dto.Role == "Student" && dto.GradeId == null)
            return Result<AuthResponseDto>.Fail("يجب تحديد الصف الدراسي / Grade is required for students");

        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Role = dto.Role,
            GradeId = dto.GradeId,
            PreferredLanguage = dto.PreferredLanguage,
            StudentCode = dto.Role == "Student" ? GenerateStudentCode() : null,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return Result<AuthResponseDto>.Fail(result.Errors.Select(e => e.Description).ToList());

        await _userManager.AddToRoleAsync(user, dto.Role);

        // Initialize student progress if student
        if (dto.Role == "Student" && dto.GradeId.HasValue)
        {
            var progress = new StudentProgress
            {
                UserId = user.Id,
                GradeId = dto.GradeId.Value,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.StudentProgress.UpsertProgressAsync(progress);
            await _unitOfWork.SaveChangesAsync();
        }

        return Result<AuthResponseDto>.Ok(await GenerateAuthResponse(user));
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByNameAsync(dto.UserName)
                   ?? await _userManager.FindByEmailAsync(dto.UserName);

        if (user == null || !user.IsActive)
            return Result<AuthResponseDto>.Fail("بيانات الدخول غير صحيحة / Invalid credentials");

        var validPassword = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!validPassword)
            return Result<AuthResponseDto>.Fail("بيانات الدخول غير صحيحة / Invalid credentials");

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result<AuthResponseDto>.Ok(await GenerateAuthResponse(user));
    }

    public async Task<Result<AuthResponseDto>> StudentPinLoginAsync(StudentPinLoginDto dto)
    {
        var users = await _unitOfWork.RefreshTokens.FindAsync(_ => false); // dummy to access context
        var user = _userManager.Users.FirstOrDefault(u => u.StudentCode == dto.StudentCode && u.IsActive);

        if (user == null)
            return Result<AuthResponseDto>.Fail("كود الطالب غير صحيح / Invalid student code");

        // Simple PIN verification using hash comparison
        if (user.PinHash == null || !VerifyPin(dto.Pin, user.PinHash))
            return Result<AuthResponseDto>.Fail("الرقم السري غير صحيح / Invalid PIN");

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        return Result<AuthResponseDto>.Ok(await GenerateAuthResponse(user));
    }

    public async Task<Result<AuthResponseDto>> FacebookLoginAsync(string accessToken)
    {
        // TODO: Validate Facebook token and get user info from Graph API
        // For now, return a placeholder
        return Result<AuthResponseDto>.Fail("Facebook login not yet configured");
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = (await _unitOfWork.RefreshTokens
            .FindAsync(r => r.Token == refreshToken))
            .FirstOrDefault();

        if (storedToken == null || !storedToken.IsActive)
            return Result<AuthResponseDto>.Fail("رمز التحديث غير صالح / Invalid refresh token");

        var user = await _userManager.FindByIdAsync(storedToken.UserId);
        if (user == null || !user.IsActive)
            return Result<AuthResponseDto>.Fail("المستخدم غير موجود / User not found");

        // Revoke old token
        storedToken.RevokedAt = DateTime.UtcNow;
        _unitOfWork.RefreshTokens.Update(storedToken);
        await _unitOfWork.SaveChangesAsync();

        return Result<AuthResponseDto>.Ok(await GenerateAuthResponse(user));
    }

    public async Task<Result<GuestSessionDto>> CreateGuestSessionAsync(GuestCreateDto dto)
    {
        var gradeExists = await _unitOfWork.Grades.ExistsAsync(g => g.Id == dto.GradeId);
        if (!gradeExists)
            return Result<GuestSessionDto>.Fail("الصف الدراسي غير موجود / Grade not found");

        var session = new GuestSession
        {
            DisplayName = dto.DisplayName,
            GradeId = dto.GradeId,
            SessionToken = GenerateRandomToken(),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        await _unitOfWork.GuestSessions.AddAsync(session);
        await _unitOfWork.SaveChangesAsync();

        return Result<GuestSessionDto>.Ok(new GuestSessionDto
        {
            SessionId = session.Id,
            DisplayName = session.DisplayName,
            GradeId = session.GradeId,
            SessionToken = session.SessionToken,
            ExpiresAt = session.ExpiresAt
        });
    }

    public async Task<Result<UserProfileDto>> GetProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result<UserProfileDto>.Fail("المستخدم غير موجود / User not found");

        string? gradeName = null;
        if (user.GradeId.HasValue)
        {
            var grade = await _unitOfWork.Grades.GetByIdAsync(user.GradeId.Value);
            gradeName = grade?.NameEn;
        }

        return Result<UserProfileDto>.Ok(new UserProfileDto
        {
            Id = user.Id,
            UserName = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            GradeId = user.GradeId,
            GradeName = gradeName,
            AvatarUrl = user.AvatarUrl,
            StudentCode = user.StudentCode,
            PreferredLanguage = user.PreferredLanguage,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        });
    }

    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileDto dto)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return Result.Fail("المستخدم غير موجود / User not found");

        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.AvatarUrl != null) user.AvatarUrl = dto.AvatarUrl;
        if (dto.PreferredLanguage != null) user.PreferredLanguage = dto.PreferredLanguage;

        if (!string.IsNullOrEmpty(dto.NewPassword) && !string.IsNullOrEmpty(dto.CurrentPassword))
        {
            var passwordResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!passwordResult.Succeeded)
                return Result.Fail(passwordResult.Errors.Select(e => e.Description).ToList());
        }

        if (!string.IsNullOrEmpty(dto.Pin))
        {
            user.PinHash = HashPin(dto.Pin);
        }

        await _userManager.UpdateAsync(user);
        return Result.Ok("تم تحديث الملف الشخصي بنجاح / Profile updated successfully");
    }

    public async Task<Result> LogoutAsync(string userId, string refreshToken)
    {
        var token = (await _unitOfWork.RefreshTokens
            .FindAsync(r => r.UserId == userId && r.Token == refreshToken))
            .FirstOrDefault();

        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokens.Update(token);
            await _unitOfWork.SaveChangesAsync();
        }

        return Result.Ok();
    }

    // ===== Private Helpers =====

    private async Task<AuthResponseDto> GenerateAuthResponse(ApplicationUser user)
    {
        var accessToken = GenerateJwtToken(user);
        var refreshToken = await GenerateAndStoreRefreshToken(user);

        return new AuthResponseDto
        {
            UserId = user.Id,
            UserName = user.UserName!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = user.Role,
            GradeId = user.GradeId,
            AvatarUrl = user.AvatarUrl,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:AccessTokenExpiryMinutes"] ?? "60")),
            PreferredLanguage = user.PreferredLanguage
        };
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.Role, user.Role),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
            new("gradeId", user.GradeId?.ToString() ?? ""),
            new("lang", user.PreferredLanguage)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _config["Jwt:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(
            int.Parse(_config["Jwt:AccessTokenExpiryMinutes"] ?? "60"));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expiry,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<string> GenerateAndStoreRefreshToken(ApplicationUser user)
    {
        var token = new RefreshToken
        {
            UserId = user.Id,
            Token = GenerateRandomToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(
                int.Parse(_config["Jwt:RefreshTokenExpiryDays"] ?? "30")),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokens.AddAsync(token);
        await _unitOfWork.SaveChangesAsync();

        return token.Token;
    }

    private static string GenerateRandomToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private static string GenerateStudentCode()
    {
        var random = new Random();
        return $"EWD-{random.Next(1000, 9999)}";
    }

    private static string HashPin(string pin)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(pin));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPin(string pin, string hash)
    {
        return HashPin(pin) == hash;
    }
}
