namespace API.DTOs;

public class SongDto
{
    public string ArtistName { get; set; }
    public string Title { get; set; }
    public List<TrackDto> Tracks { get; set; }
}