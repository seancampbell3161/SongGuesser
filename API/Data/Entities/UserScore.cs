using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace API.Data.Entities;

public class UserScore
{
    public int Id { get; init; }
    [StringLength(50)]
    public string UserId { get; init; } = string.Empty;
    public int Score { get; init; }
    public int NumOfGuesses { get; init; }
    public DateTime CreatedUtc { get; init; } = DateTime.UtcNow;
    
    public ApplicationUser? User { get; init; }
}