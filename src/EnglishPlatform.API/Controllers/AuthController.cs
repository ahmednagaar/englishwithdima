using EnglishPlatform.Application.DTOs.Auth;
using EnglishPlatform.Application.Interfaces;
using EnglishPlatform.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnglishPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        return result.Success ? Ok(ApiResponse<AuthResponseDto>.Ok(result.Data!)) : BadRequest(ApiResponse<AuthResponseDto>.Fail(result.Errors));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result.Success ? Ok(ApiResponse<AuthResponseDto>.Ok(result.Data!)) : Unauthorized(ApiResponse<AuthResponseDto>.Fail(result.Errors));
    }

    [HttpPost("student-login")]
    public async Task<IActionResult> StudentPinLogin([FromBody] StudentPinLoginDto dto)
    {
        var result = await _authService.StudentPinLoginAsync(dto);
        return result.Success ? Ok(ApiResponse<AuthResponseDto>.Ok(result.Data!)) : Unauthorized(ApiResponse<AuthResponseDto>.Fail(result.Errors));
    }

    [HttpPost("facebook")]
    public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginDto dto)
    {
        var result = await _authService.FacebookLoginAsync(dto.AccessToken);
        return result.Success ? Ok(ApiResponse<AuthResponseDto>.Ok(result.Data!)) : BadRequest(ApiResponse<AuthResponseDto>.Fail(result.Errors));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken);
        return result.Success ? Ok(ApiResponse<AuthResponseDto>.Ok(result.Data!)) : Unauthorized(ApiResponse<AuthResponseDto>.Fail(result.Errors));
    }

    [HttpPost("guest")]
    public async Task<IActionResult> CreateGuestSession([FromBody] GuestCreateDto dto)
    {
        var result = await _authService.CreateGuestSessionAsync(dto);
        return result.Success ? Ok(ApiResponse<GuestSessionDto>.Ok(result.Data!)) : BadRequest(ApiResponse<GuestSessionDto>.Fail(result.Errors));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _authService.GetProfileAsync(userId);
        return result.Success ? Ok(ApiResponse<UserProfileDto>.Ok(result.Data!)) : NotFound(ApiResponse<UserProfileDto>.Fail(result.Errors));
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await _authService.UpdateProfileAsync(userId, dto);
        return result.Success ? Ok(ApiResponse<string>.Ok("Profile updated")) : BadRequest(ApiResponse<string>.Fail(result.Errors));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _authService.LogoutAsync(userId, dto.RefreshToken);
        return Ok(ApiResponse<string>.Ok("Logged out successfully"));
    }
}
