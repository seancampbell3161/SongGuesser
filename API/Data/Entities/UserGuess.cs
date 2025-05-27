using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities;

public class UserGuess
{
    public int Id { get; init; }
    [ForeignKey("User")]
    public string UserId { get; set; }
    public string Guess { get; set; }
    [ForeignKey("Song")]
    public int SongId { get; set; }
    public DateTime CreatedUtc { get; set; }

    public ApplicationUser User { get; set; }
    public Song Song { get; set; }
}