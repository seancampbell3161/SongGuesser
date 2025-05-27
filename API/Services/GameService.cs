using API.DTOs;
using API.Interfaces;

namespace API.Services;

public class GameService(IGameRepository gameRepository) : IGameService
{
    public async Task<bool> ProcessUserGuessAsync(UserGuessDto userGuess)
    {
        var addGuess = await gameRepository.AddUserGuessAsync(userGuess);

        if (addGuess == null) return false;
        
        var isCorrect = await ValidateGuessAsync(userGuess.Guess);

        switch (isCorrect)
        {
            case false when addGuess != 4:
                return isCorrect;
            case false:
                await gameRepository.AddUserScoreAsync(new UserResultDto(userGuess.UserId, 0, (int)addGuess));
                return isCorrect;
            case true:
                var score = CalculateUserScore((int)addGuess);
                await gameRepository.AddUserScoreAsync(new UserResultDto(userGuess.UserId, score, (int)addGuess));
                return isCorrect;
        }
    }

    private static int CalculateUserScore(int userGuessGuessNumber)
    {
        return 100 - 25 * (userGuessGuessNumber - 1);
    }

    private async Task<bool> ValidateGuessAsync(string guess)
    {
        var songOfTheDay = await gameRepository.GetSongOfTheDayAsync();

        if (songOfTheDay == null) return false;

        var isCorrect = string.Equals(guess.Trim(), songOfTheDay.SongTitle, StringComparison.CurrentCultureIgnoreCase)
                        || string.Equals(guess.Trim(), songOfTheDay.Artist, StringComparison.CurrentCultureIgnoreCase);

        return isCorrect;
    }
}