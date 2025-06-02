using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class LoginDto
{
    [StringLength(100)] public string Email { get; set; } = "";
    [StringLength(100)] public string Password { get; set; } = "";
    public bool RememberMe { get; set; }
}