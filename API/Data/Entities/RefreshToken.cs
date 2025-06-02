using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Data.Entities;

public class RefreshToken
{
    public int Id { get; init; }
    [StringLength(50)] public string Token { get; init; } = "";
    [ForeignKey("ApplicationUser")]
    [StringLength(50)] 
    public string UserId { get; init; } = "";
    public DateTime ExpiresUtc { get; init; }
    public DateTime CreatedUtc { get; init; }
    public bool IsRevoked { get; set; }

    public ApplicationUser ApplicationUser { get; set; }
}