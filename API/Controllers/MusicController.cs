using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController(
    IAudioService audioService,
    ISongRepository songRepository,
    IGameRepository gameRepository)
    : ControllerBase
{
    [HttpGet("song")]
    public async Task<IActionResult> GetSongOfTheDay()
    {
        var songId = await gameRepository.GetSongOfTheDaySongIdAsync();

        if (songId == 0) return Ok(null);

        var song = await songRepository.GetSongAsync(songId);

        if (song == null) return Ok(null);

        song.Tracks = OrderTracks(song);

        return Ok(song);
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandomSongTracks()
    {
        try
        {
            var response = await songRepository.GetRandomSongAsync();

            if (response == null)
            {
                return Ok(null);
            }

            response.Tracks = OrderTracks(response);
            
            return Ok(response);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
    
    [Authorize]
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertAsync([FromForm] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("No YouTube URL provided.");
        }

        var result = await audioService.ConvertToMp3Async(url);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("separate")]
    public async Task<IActionResult> SeparateTracksAsync([FromForm] IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var result = await audioService.SeparateTracksAsync(file);

        return Ok(result);
    }

    [HttpPost("separate-result")]
    public async Task<IActionResult> SeparateTracksFromResultDto([FromBody] ConvertResult convertResult)
    {
        if (string.IsNullOrWhiteSpace(convertResult.FilePath)) return BadRequest("Url required");

        var result = await audioService.SeparateTracksAsync(convertResult);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("convert-and-separate")]
    public async Task<IActionResult> ConvertAndSeparateAsync([FromBody] YouTubeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url)) return BadRequest("You must enter a URL");
        if (string.IsNullOrWhiteSpace(request.SongTitle)) return BadRequest("You must enter a song name");
        if (string.IsNullOrWhiteSpace(request.Artist)) return BadRequest("You must enter an Artist name");

        var result = await audioService.ConvertAndSeparateTracksAsync(request.Url);

        if (result.Error != null)
        {
            return StatusCode(500);
        }

        try
        {
            await songRepository.AddSongAsync(request, result);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }

        return Ok(result);
    }
    
    private static List<TrackDto> OrderTracks(SongDto song)
    {
        List<TrackDto> ordered = [];
            
        ordered.Add(song.Tracks.First(x => x.Name.Contains("drum")));
        ordered.Add(song.Tracks.First(x => x.Name.Contains("bass")));
        ordered.Add(song.Tracks.First(x => x.Name.Contains("other")));
        ordered.Add(song.Tracks.First(x => x.Name.Contains("vocal")));

        return ordered;
    }
}