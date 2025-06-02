using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Data.Entities;
using API.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext context,
    IConfiguration configuration)
    : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data");
        
        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            // code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            //
            // var callbackUrl = Url.Page(
            //     "/Account/ConfirmEmail",
            //     pageHandler: null,
            //     values: new { area = "Identity", userId = user.Id, code = code },
            //     protocol: Request.Scheme)!;
            //
            // await emailSender.SendConfirmationLinkAsync(
            //     user,
            //     "Confirm your email",
            //     $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            //
            // if (userManager.Options.SignIn.RequireConfirmedAccount)
            // {
            //     return RedirectToPage("RegisterConfirmation", 
            //         new { email = model.Email });
            // }

            await signInManager.SignInAsync(user, isPersistent: false);

            var authToken = GenerateJwtToken(user);
            var refreshToken = await GenerateRefreshToken(user);
            
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMonths(1)
            });
            
            return Ok(new { Token = authToken});
        }

        return BadRequest(result.Errors);
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

        if (!result.Succeeded)
            return Unauthorized(result.IsLockedOut ? "Account locked out." : "Invalid login attempt");
        
        var authToken = GenerateJwtToken(user);
        var refreshToken = await GenerateRefreshToken(user);
        
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMonths(1)
        });
        
        return Ok(new { Token = authToken });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refreshToken", out var incomingRefreshToken))
            return Unauthorized(new { error = "no_refresh_token" });

        var existingToken = await context.RefreshTokens
            .OrderByDescending(rt => rt.CreatedUtc)
            .Include(rt => rt.ApplicationUser)
            .FirstOrDefaultAsync(rt => rt.Token == incomingRefreshToken);

        if (existingToken == null || existingToken.IsRevoked || existingToken.ExpiresUtc < DateTime.UtcNow)
            return Unauthorized(new { error = "invalid_refresh_token" });

        existingToken.IsRevoked = true;
        context.RefreshTokens.Update(existingToken);

        var user = existingToken.ApplicationUser;
        
        var authToken = GenerateJwtToken(user);
        var refreshToken = await GenerateRefreshToken(user);
        
        Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMonths(1)
        });

        return Ok(new { Token = authToken });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            return string.Empty;
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private async Task<string> GenerateRefreshToken(ApplicationUser user)
    {
        var tokenValue = Guid.NewGuid().ToString("N")
                         + Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        context.RefreshTokens.Add(new RefreshToken
        {
            Token = tokenValue,
            UserId = user.Id,
            ExpiresUtc = DateTime.UtcNow.AddMonths(1),
            CreatedUtc = DateTime.UtcNow
        });

        await context.SaveChangesAsync();

        return tokenValue;
    }
}