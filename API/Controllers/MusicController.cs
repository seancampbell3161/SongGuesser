using API.Data;
using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using FFMpegCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class MusicController(
    IAudioService audioService,
    ApplicationDbContext context)
    : ControllerBase
{
    [HttpGet("random")]
    public async Task<IActionResult> GetRandomSongTracks()
    {
        try
        {
            var songIds = await context.Songs.Select(s => s.Id).ToListAsync();

            if (songIds.Count == 0)
            {
                return NotFound(); 
            }

            var random = new Random();
            var randomSongId = songIds[random.Next(songIds.Count)];

            var song = await context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Tracks)
                .FirstOrDefaultAsync(s => s.Id == randomSongId);

            if (song == null)
            {
                return NotFound();
            }

            var response = new
            {
                id = song.Id,
                title = song.Title,
                artist = song.Artist.Name,
                tracks = song.Tracks.Select(t => new { t.Name, t.Path })
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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

        if (result.Error == null)
        {
            try
            {
                var artist = await context.Artists
                    .FirstOrDefaultAsync(a => a.Name == request.Artist.ToUpper().Trim());

                if (artist == null)
                {
                    artist = new Artist
                    {
                        Name = request.Artist.ToUpper().Trim()
                    };

                    context.Artists.Add(artist);
                }

                var song = new Song
                {
                    Title = request.SongTitle.ToUpper().Trim(),
                    ArtistId = artist.Id
                };

                context.Songs.Add(song);

                foreach (var track in result.Tracks.Select(trackResult => new Track
                         {
                             Name = trackResult.Name,
                             Path = trackResult.Path,
                             SongId = song.Id
                         }))
                {
                    context.Tracks.Add(track);
                }

                await context.SaveChangesAsync();
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