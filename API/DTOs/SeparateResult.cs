namespace API.DTOs;

public class SeparateResult
{
    public string Message { get; set; }
    public List<Track> Tracks { get; set; }
    public string Error { get; set; }
}