using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace API.Data.Repositories;

public class GameRepository(ApplicationDbContext context) : IGameRepository
{
    public async Task AddUserScoreAsync(UserResultDto result)
    {
        try
        {
            context.UserScores.Add(new UserScore
            {
                UserId = result.UserId,
                Score = result.Score,
                NumOfGuesses = result.NumOfGuesses
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
                .OrderByDescending(x => x.TotalScore)
                .Take(5)
                .ToListAsync();

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return [];
        }
    }

    public async Task<SongOfTheDay?> GetSongOfTheDayAsync()
    {
        var entity = await context.SongsOfTheDay
            .FirstOrDefaultAsync(x => x.CreatedUtc.Day == DateTime.UtcNow.Day);

        return entity;
    }

    public async Task AddNewSongOfTheDayAsync()
    {
        try
        {
            var numOfSongs = context.Songs.Count();

            Random rand = new();
            var randomId = rand.Next(1, numOfSongs);

            var song = await context.Songs
                .Include(s => s.Artist)
                .Select(s => new
                {
                    Id = s.Id,
                    Song = s.Title,
                    Artist = s.Artist != null ? s.Artist.Name : string.Empty
                })
                .FirstOrDefaultAsync(s => s.Id == randomId);

            if (song == null) return;

            context.SongsOfTheDay.Add(new SongOfTheDay
            {
                Artist = song.Artist,
                Song = song.Song,
                CreatedUtc = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}