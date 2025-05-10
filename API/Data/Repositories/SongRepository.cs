using API.Data.Entities;
using API.DTOs;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using WaveformData = API.Data.Entities.WaveformData;

namespace API.Data.Repositories;

public class SongRepository(ApplicationDbContext context) : ISongRepository
{
    public async Task AddSongAsync(YouTubeRequest request, SeparateResult result)
    {
        try
        {
            var artist = await context.Artists.Include(artist => artist.Songs)
                .FirstOrDefaultAsync(a => a.Name == request.Artist!.ToUpper().Trim());

            if (artist == null)
            {
                artist = new Artist
                {
                    Name = request.Artist!.ToUpper().Trim(),
                    Songs = [ new Song
                    {
                        Title = request.SongTitle!.ToUpper().Trim(),
                        Tracks = result.Tracks.Select(t => new Track
                        {
                            Name = t.Name,
                            Path = t.Path,
                            WaveformData = new WaveformData
                            {
                                Id = 0
                            },
                            WaveformId = 0
                        }).ToList()
                    }]
                };

                context.Artists.Add(artist);
                
                // foreach (var track in result.Tracks.Select(trackResult => new Track
                //          {
                //              Name = trackResult.Name,
                //              Path = trackResult.Path,
                //              SongId = artist.Songs
                //                  .Where(x => x.Title == request.SongTitle!.ToUpper().Trim()).FirstOrDefault()!.Id
                //          }))
                // {
                //     context.Tracks.Add(track);
                // }
            }
            else
            {
                var song = new Song
                {
                    Title = request.SongTitle!.ToUpper().Trim(),
                    ArtistId = artist.Id
                };

                context.Songs.Add(song);
                
                foreach (var track in result.Tracks.Select(trackResult => new Track
                         {
                             Name = trackResult.Name,
                             Path = trackResult.Path,
                             SongId = song.Id,
                             WaveformData = new WaveformData
                             {
                                 Id = 0
                             },
                             WaveformId = 0
                         }))
                {
                    context.Tracks.Add(track);
                }
            }
            
            await context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
    }

    public async Task<SongDto> GetSongAsync(int songId)
    {
        try
        {
            var song = await context.Songs
                .Include(s => s.Artist)
                .Include(s => s.Tracks)
                .FirstOrDefaultAsync(s => s.Id == songId);

            if (song == null)
            {
                throw new KeyNotFoundException();
            }

            return new SongDto
            {
                ArtistName = song.Artist.Name,
                Title = song.Title,
                Tracks = song.Tracks.Select(x => new TrackDto { Name = x.Name, Path = x.Path }).ToList()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
    }

    public async Task<SongDto> GetRandomSongAsync()
    {
        try
        {
            var songIds = await context.Songs.Select(s => s.Id).ToListAsync();

            if (songIds.Count == 0)
            {
                Console.WriteLine("No songs available");
                return new SongDto();
            }

            var random = new Random();
            var randomSongId = songIds[random.Next(songIds.Count)];

            return await GetSongAsync(randomSongId);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw new Exception(ex.Message);
        }
    }
}