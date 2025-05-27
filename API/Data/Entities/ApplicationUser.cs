using Microsoft.AspNetCore.Identity;

namespace API.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<UserGuess> UserGuesses { get; init; } = [];
    public ICollection<UserScore> UserScores { get; init; } = [];
}