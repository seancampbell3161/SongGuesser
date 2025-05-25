using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities;

public class SongOfTheDay
{
    public int Id { get; init; }
    [StringLength(100)]
    public string Artist { get; init; } = string.Empty;
    [ForeignKey("Song")]
    public int SongId { get; init; } = 1;
    [StringLength(150)]
    public string SongTitle { get; init; } = string.Empty;
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;

    public Song Song { get; set; }
}