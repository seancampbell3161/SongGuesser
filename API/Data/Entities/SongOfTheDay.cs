using System.ComponentModel.DataAnnotations;

namespace API.Data.Entities;

public class SongOfTheDay
{
    public int Id { get; init; }
    [StringLength(100)]
    public string Artist { get; init; } = string.Empty;
    [StringLength(150)]
    public string Song { get; init; } = string.Empty;
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
}