using Microsoft.AspNetCore.Identity;

namespace API.Data.Entities;

public class UserScore
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int Score { get; set; }
    public int NumOfGuesses { get; set; }
    
    public ApplicationUser? User { get; set; }
}