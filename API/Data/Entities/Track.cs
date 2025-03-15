using System.ComponentModel.DataAnnotations;

namespace API.Data.Entities;

public class Track
{
    public int Id { get; set; }
    [StringLength(150)]
    public string Name { get; set; } = "";
    [StringLength(200)]
    public string Path { get; set; } = "";
    public int SongId { get; set; }
    public int WaveformId { get; set; }
    public Song? Song { get; set; }
    public WaveformData? WaveformData { get; set; }
}