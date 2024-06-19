namespace API.Data.Entities;

public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int ArtistId { get; set; }
    public Artist Artist { get; set; }

    public ICollection<Track> Tracks { get; set; }
}