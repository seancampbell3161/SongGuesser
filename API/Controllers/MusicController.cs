using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class MusicController : ControllerBase
{
    private readonly IAudioService _audioService;

    public MusicController(IAudioService audioService)
    {
        _audioService = audioService;
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandomSongTracks()
    {
        var result = await _audioService.GetRandomSongTracksAsync();
        return Ok(result);
    }
    
    [HttpPost("convert")]
    public async Task<IActionResult> ConvertAsync([FromForm] string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return BadRequest("No YouTube URL provided.");
        }

        var result = await _audioService.ConvertToMp3Async(url);
        return Ok(result);
    }
    
    [HttpPost("separate")]
    public async Task<IActionResult> SeparateTracksAsync([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var result = await _audioService.SeparateTracksAsync(file);

        return Ok(result);
    }

    [HttpPost("separate-result")]
    public async Task<IActionResult> SeparateTracksFromResultDto([FromBody] ConvertResult convertResult)
    {
        if (string.IsNullOrWhiteSpace(convertResult.FilePath)) return BadRequest("Url required");
        
        var result = _audioService.SeparateTracksAsync(convertResult);
        return Ok(result);
    }

    [HttpPost("convert-and-separate")]
    public async Task<IActionResult> ConvertAndSeparateAsync([FromBody] YouTubeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("URL cannot be null");
        }

        var result = await _audioService.ConvertAndSeparateTracksAsync(request.Url);
        return Ok(result);
    }
}