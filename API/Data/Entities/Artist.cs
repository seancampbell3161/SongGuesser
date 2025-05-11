using System.ComponentModel.DataAnnotations;

namespace API.Data.Entities;

public class Artist
{
    public int Id { get; init; }
    [StringLength(100)]
    public string Name { get; init; } = "";

    public ICollection<Song> Songs { get; init; } = new List<Song>();
}