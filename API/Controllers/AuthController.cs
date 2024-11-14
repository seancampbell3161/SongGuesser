using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Data.Entities;
using API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (ModelState.IsValid)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered successfully" });
            }

            return BadRequest(result.Errors);
        }

        return BadRequest("Invalid data");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized("Invalid login attempt: user not found");
        }

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        if (result.IsLockedOut)
        {
            return Unauthorized("Account locked out.");
        }

        return Unauthorized("Invalid login attempt");
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: null,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}