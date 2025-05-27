using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class GameRepository(ApplicationDbContext context) : IGameRepository
{
    public async Task<int?> AddUserGuessAsync(UserGuessDto guess)
    {
        try
        {
            var existingNumOfGuesses = await context.UserGuesses
                .Where(x => x.UserId == guess.UserId
                            && x.SongId == guess.SongId
                            && x.CreatedUtc.Day == DateTime.UtcNow.Day)
                .CountAsync();

            if (existingNumOfGuesses == 4) return null;

            context.UserGuesses.Add(new UserGuess
            {
                UserId = guess.UserId,
                SongId = guess.SongId,
                Guess = guess.Guess,
                CreatedUtc = DateTime.UtcNow
            });

            await context.SaveChangesAsync();

            return existingNumOfGuesses + 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }

        return null;
    }

    public async Task AddUserScoreAsync(UserResultDto result)
    {
        // TODO remove try catches once setup middleware
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
                    userScore.UserId,
                    user.UserName,
                    userScore.Score,
                    userScore.NumOfGuesses
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
                .Where(x => x.Score > 0)
                .Join(context.Users, userScore => userScore.UserId, user => user.Id, (userScore, user) => new
                {
                    userScore.UserId,
                    user.UserName,
                    userScore.Score
                })
                .GroupBy(x => x.UserName)
                .Take(10)
                .Select(x => new UserScoreDto
                {
                    UserName = x.Key ?? "",
                    TotalScore = x.Sum(y => y.Score)
                })
                .OrderByDescending(x => x.TotalScore)
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

    public async Task<int> GetSongOfTheDaySongIdAsync()
    {
        var entity = await context.SongsOfTheDay
            .FirstOrDefaultAsync(x => x.CreatedUtc.Day == DateTime.UtcNow.Day);

        return entity?.SongId ?? 0;
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
                    s.Id,
                    Song = s.Title,
                    Artist = s.Artist != null ? s.Artist.Name : string.Empty
                })
                .FirstOrDefaultAsync(s => s.Id == randomId);

            if (song == null) return;

            context.SongsOfTheDay.Add(new SongOfTheDay
            {
                Artist = song.Artist,
                SongId = song.Id,
                SongTitle = song.Song,
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