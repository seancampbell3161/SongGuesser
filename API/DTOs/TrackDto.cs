namespace API.DTOs;

public class TrackDto
{
    public string Name { get; set; }
    public string Path { get; set; }
    // public ICollection<WaveformData> WaveformData { get; set; } = [];
    public string? WaveformData { get; set; }
}
