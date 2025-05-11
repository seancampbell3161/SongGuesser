using System.ComponentModel.DataAnnotations;

namespace API.Data.Entities;

public class Track
{
    public int Id { get; init; }
    [StringLength(150)]
    public string Name { get; init; } = "";
    [StringLength(200)]
    public string Path { get; init; } = "";
    public int SongId { get; init; }
    public int WaveformId { get; init; }
    public Song? Song { get; init; }
    public WaveformData? WaveformData { get; init; }
}