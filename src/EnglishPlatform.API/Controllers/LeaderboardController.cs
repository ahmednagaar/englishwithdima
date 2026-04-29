using EnglishPlatform.Infrastructure.Repositories.Interfaces;
using EnglishPlatform.Shared;
using Microsoft.AspNetCore.Mvc;

namespace EnglishPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly ILeaderboardRepository _leaderboardRepo;

    public LeaderboardController(ILeaderboardRepository leaderboardRepo) =>
        _leaderboardRepo = leaderboardRepo;

    /// <summary>
    /// Get top students for a specific grade.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetLeaderboard([FromQuery] int gradeId = 1, [FromQuery] int count = 50)
    {
        if (gradeId < 1 || gradeId > 20)
            return BadRequest(ApiResponse<string>.Fail("Invalid grade ID"));

        if (count < 1 || count > 100)
            count = 50;

        var entries = await _leaderboardRepo.GetGradeTopAsync(gradeId, count);

        var result = entries.Select((e, i) => new
        {
            userId = e.UserId,
            displayName = e.DisplayName,
            avatarUrl = e.AvatarUrl,
            totalPoints = e.TotalPoints,
            rank = e.Rank > 0 ? e.Rank : i + 1,
            badgeCount = 0 // TODO: join with UserBadges count
        }).ToList();

        return Ok(ApiResponse<object>.Ok(result));
    }
}
