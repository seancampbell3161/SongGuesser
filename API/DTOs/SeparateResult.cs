namespace API.DTOs;

public class SeparateResult
{
    public string? Message { get; set; }
    public List<TrackDto> Tracks { get; set; } = [];
    public string? Error { get; set; }
}