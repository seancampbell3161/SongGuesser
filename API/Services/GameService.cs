using API.DTOs;
using API.Interfaces;

namespace API.Services;

public class GameService(IGameRepository gameRepository) : IGameService
{
    public async Task<bool> ProcessUserGuessAsync(UserGuessDto userGuess)
        {
        var isCorrect = await ValidateGuessAsync(userGuess.Guess);

        switch (isCorrect)
        {
            case false when userGuess.GuessNumber != 4:
                return isCorrect;
            case false:
                await gameRepository.AddUserScoreAsync(new UserResultDto(userGuess.UserId, 0, userGuess.GuessNumber));
                return isCorrect;
            case true:
                var score = CalculateUserScore(userGuess.GuessNumber);
                await gameRepository.AddUserScoreAsync(new UserResultDto(userGuess.UserId, score, userGuess.GuessNumber));
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