using System.Text.RegularExpressions;
using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class UserScoreRepository(ApplicationDbContext context) : IUserScoreRepository
{
    public async Task AddUserScoreAsync(UserResultDto userResult, string userId)
    {
        try
        {
            context.UserScores.Add(new UserScore
            {
                Id = 0,
                UserId = userId,
                Score = userResult.CorrectlyAnswered ? 100 - 25 * (userResult.NumOfGuesses - 1) : 0,
                NumOfGuesses = userResult.NumOfGuesses
            });

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task<UserScoreDto> GetUserScoreAsync(string userId)
    {
        try
        {
            var result = await context.UserScores
                .Join(context.Users, userScore => userScore.UserId, user => user.Id, (userScore, user) => new
                {
                    UserId = userScore.UserId,
                    UserName = user.UserName,
                    Score = userScore.Score,
                    NumOfGuesses = userScore.NumOfGuesses
                })
                .Where(x => x.UserId == userId)
                .GroupBy(x => x.UserId)
                .Select(group => new UserScoreDto
                {
                    UserName = group.Select(x => x.UserName).FirstOrDefault() ?? "",
                    TotalScore = group.Sum(x => x.Score),
                    NumOfGames = group.Count()
                }).FirstOrDefaultAsync();

            return result ?? new UserScoreDto();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new UserScoreDto();
        }
    }

    public async Task<List<UserScoreDto>> GetHighScoresAsync()
    {
        try
        {
            var result = await context.UserScores
                .Join(context.Users, userScore => userScore.UserId, user => user.Id, (userScore, user) => new
                {
                    UserId = userScore.UserId,
                    UserName = user.UserName,
                    Score = userScore.Score
                })
                .GroupBy(x => x.UserName)
                .Select(x => new UserScoreDto
                {
                    UserName = x.Key ?? "",
                    TotalScore = x.Sum(y => y.Score)
                })
                .OrderByDescending(x => x.TotalScore).ToListAsync();

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return [];
        }
    }
}