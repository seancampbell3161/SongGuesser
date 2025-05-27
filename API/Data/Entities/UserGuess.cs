using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities;

public class UserGuess
{
    public int Id { get; init; }
    [ForeignKey("User")]
    [StringLength(40)]
    public string UserId { get; init; } = "";
    [StringLength(100)]
    public string Guess { get; init; } = "";
    [ForeignKey("Song")]
    public int SongId { get; init; }
    public DateTime CreatedUtc { get; init; }

    public ApplicationUser User { get; init; }
    public Song Song { get; init; }
}