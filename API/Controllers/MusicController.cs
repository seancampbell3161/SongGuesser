using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class MusicController(
    IAudioService audioService,
    ISongRepository songRepository)
    : ControllerBase
{
    [HttpGet("random")]
    public async Task<IActionResult> GetRandomSongTracks()
    {
        try
        {
            var response = await songRepository.GetRandomSongAsync();

            return Ok(response);
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

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

    [HttpPost("convert-and-separate")]
    public async Task<IActionResult> ConvertAndSeparateAsync([FromBody] YouTubeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Url))
        {
            return BadRequest("URL cannot be null");
        }

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
}