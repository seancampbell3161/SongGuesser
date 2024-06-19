namespace API.Data.Entities;

public class Track
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Path { get; set; } = "";
    public int SongId { get; set; }
    public Song? Song { get; set; }
}