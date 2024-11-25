using API.DTOs;

namespace API.Interfaces;

public interface IUserScoreRepository
{
    Task AddUserScoreAsync(UserResultDto userResult, string userId);
    Task<UserScoreDto> GetUserScoreAsync(string userId);
    Task<List<UserScoreDto>> GetHighScoresAsync();
}