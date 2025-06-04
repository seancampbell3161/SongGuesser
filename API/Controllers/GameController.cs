using API.Data;
using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Authorize]
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
            var score = await context.UserScores
                .Where(x => x.UserId == user.Id)
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
    
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> SubmitGuess(UserGuessDto guess)
    {
        var user = await userManager.GetUserAsync(HttpContext.User);
        
        if (user == null)
        {
            var result = await gameService.ProcessAnonymousUserGuessAsync(guess);
            return Ok(result);
        }

        try
        {
            var result = await gameService.ProcessUserGuessAsync(guess with { UserId = user.Id });
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(409, ex.Message);
        }
    }

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
    {
        var topScores = await gameRepository.GetHighScoresAsync();
        return Ok(topScores);
    }
}