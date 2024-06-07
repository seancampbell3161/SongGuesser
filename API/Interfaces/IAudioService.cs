using API.DTOs;

namespace API.Interfaces;

public interface IAudioService
{
    Task<ConvertResult> ConvertToMp3Async(string url);
    Task<SeparateResult> SeparateTracksAsync(IFormFile file);
    Task<SeparateResult> SeparateTracksAsync(ConvertResult result);
    Task<SeparateResult> ConvertAndSeparateTracksAsync(string url);
    Task<SeparateResult> GetRandomSongTracksAsync();
}