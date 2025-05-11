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
    IGameRepository gameRepository,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context,
    IGameService gameService)
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
    public async Task<IActionResult> SubmitGuess(UserGuessDto guess)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        if (user == null)
        {
            return Unauthorized();
        }

        var result = await gameService.ProcessUserGuessAsync(guess with { UserId = user.Id });
        
        return Ok(result);
    }

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
    {
        var topScores = await gameRepository.GetHighScoresAsync();
        return Ok(topScores);
    }
}