using Microsoft.AspNetCore.Identity;

namespace API.Data.Entities;

public class ApplicationUser : IdentityUser
{
    public ICollection<UserScore> UserScores { get; set; } = [];
}