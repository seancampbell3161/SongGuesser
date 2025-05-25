using API.DTOs;

namespace API.Interfaces;

public interface ISongRepository
{
    Task AddSongAsync(YouTubeRequest request, SeparateResult separateResult);
    Task<SongDto?> GetSongAsync(int songId);
    Task<SongDto?> GetRandomSongAsync();
}