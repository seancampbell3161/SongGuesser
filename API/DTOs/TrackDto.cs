namespace API.DTOs;

public class TrackDto
{
    public string Name { get; set; } = string.Empty;

    public string Path { get; set; } = string.Empty;
    // public ICollection<WaveformData> WaveformData { get; set; } = [];
    public string? WaveformData { get; set; }
}
