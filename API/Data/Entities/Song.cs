using System.ComponentModel.DataAnnotations;

namespace API.Data.Entities;

public class Song
{
    public int Id { get; init; }
    [StringLength(150)]
    public string Title { get; init; } = "";
    public int ArtistId { get; init; }
    public Artist? Artist { get; init; }

    public ICollection<Track> Tracks { get; init; } = [];
}