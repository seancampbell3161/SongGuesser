using API.Data;
using API.Data.Entities;
using API.DTOs;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context)
    : ControllerBase
{
    [HttpGet("score")]
    public async Task<IActionResult> GetUserScore()
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        
        if (user == null) return Unauthorized();

        try
        {
            var score = await context.UserScores.Where(x => x.UserId == user.Id)
                .Select(x => x.Score)
                .SumAsync();
            return Ok(new UserTotalScoreDto
            {
                TotalScore = score
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
    
    [HttpPost("score")]
    public async Task<IActionResult> SubmitAnswer(UserResultDto userResult)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            return Unauthorized();
        }

        try
        {
            context.UserScores.Add(new UserScore
            {
                Id = 0,
                UserId = user.Id,
                Score = CalculateScore(userResult),
                NumOfGuesses = userResult.NumOfGuesses,
                User = user
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        return Ok();
    }

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
    {
        try
        {
            var leaders = await context.UserScores
                                                            .GroupBy(x => x.UserId)
                                                            .Select(group => new
                                                            {
                                                                UserId = group.Key,
                                                                TotalScore = group.Sum(x => x.Score)
                                                            })
                                                            .OrderByDescending(x => x.TotalScore)
                                                            .Take(5)
                                                            .ToListAsync();

            var result = new List<UserTotalScoreDto>();
            
            foreach (var leader in leaders)
            {
                var username = await context.Users
                                        .Where(x => x.Id == leader.UserId)
                                        .Select(x => x.UserName)
                                        .FirstOrDefaultAsync();
                result.Add(new UserTotalScoreDto
                {
                    UserName = username,
                    TotalScore = leader.TotalScore
                });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
        return Ok();
    }

    private int CalculateScore(UserResultDto userResult)
    {
        if (userResult.CorrectlyAnswered != true)
        {
            return 0;
        }

        return userResult.NumOfGuesses switch
        {
            1 => 100,
            2 => 75,
            3 => 50,
            4 => 25,
            _ => 0
        };
    }
}