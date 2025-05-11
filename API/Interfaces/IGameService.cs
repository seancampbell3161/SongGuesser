using API.DTOs;

namespace API.Interfaces;

public interface IGameService
{
    Task<bool> ProcessUserGuessAsync(UserGuessDto userGuess);
}