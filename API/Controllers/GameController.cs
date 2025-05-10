using API.Data;
using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController(
    IUserScoreRepository userScoreRepository,
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

            await context.SaveChangesAsync();
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
        var topScores = await userScoreRepository.GetHighScoresAsync();
        return Ok(topScores);
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