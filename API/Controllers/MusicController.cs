using API.Data;
using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class MusicController : ControllerBase
{
    private readonly IAudioService _audioService;
    private readonly ApplicationDbContext _context;

    public MusicController(IAudioService audioService, ApplicationDbContext context)
    {
        _audioService = audioService;
        _context = context;
    }

    [HttpGet("random")]
    public async Task<IActionResult> GetRandomSongTracks()
    {
        try
        {
            var song = await _context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Tracks)
                .OrderBy(r => Guid.NewGuid())
                .FirstOrDefaultAsync();

            if (song != null)
            {
                var response = new
                {
                    songId = song.Id,
                    title = song.Title,
                    artist = song.Artist.Name,
                    tracks = song.Tracks.Select(t => new { t.Name, t.Path })
                };

                return Ok(response);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return StatusCode(500);
        }

        return NotFound();
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

        if (result.Error == null)
        {
            try
            {
                var artist = await _context.Artists
                    .FirstOrDefaultAsync(a => a.Name == request.Artist.ToUpper().Trim());

                if (artist == null)
                {
                    artist = new Artist
                    {
                        Name = request.Artist.ToUpper().Trim()
                    };

                    await _context.Artists.AddAsync(artist);
                    await _context.SaveChangesAsync();
                }

                var song = new Song
                {
                    Title = request.SongTitle.ToUpper().Trim(),
                    ArtistId = artist.Id
                };

                await _context.Songs.AddAsync(song);
                await _context.SaveChangesAsync();

                foreach (var trackResult in result.Tracks)
                {
                    var track = new Track
                    {
                        Name = trackResult.Name,
                        Path = trackResult.Path,
                        SongId = song.Id
                    };

                    await _context.Tracks.AddAsync(track);
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500);
            }
        }
        return Ok(result);
    }
}