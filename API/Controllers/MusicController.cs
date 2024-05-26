using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MusicController : ControllerBase
{
    private readonly IAudioService _audioService;

    public MusicController(IAudioService audioService)
    {
        _audioService = audioService;
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

    [HttpPost("convert-and-separate")]
    public async Task<IActionResult> ConvertAndSeparateAsync(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest("URL cannot be null");
        }

        var result = _audioService.ConvertAndSeparateTracksAsync(url);
        return Ok(result);
    }
}