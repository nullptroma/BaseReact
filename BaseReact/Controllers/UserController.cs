using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using BaseReact.Database;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BaseReact.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController
{
    private AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Route("/login")]
    public IResult Login(PersonInput person)
    {
        DbUser? dbUser = _context.Users.FirstOrDefault(dbu => dbu.Email == person.Email);
        if(dbUser == null || dbUser.HashedPassword != HashString(person.Password, dbUser.Salt))
            return Results.Unauthorized();
        return GenerateToken(person);
    }

    [HttpPost]
    [Route("/register")]
    public async Task<IResult> Register(PersonInput person)
    {
        bool exists = _context.Users.Any(dbu => dbu.Email == person.Email);
        if (exists)
            return Results.Conflict();

        string userSalt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        _context.Users.Add(new DbUser
        {
            Email = person.Email,
            HashedPassword = HashString(person.Password, userSalt),
            Salt = userSalt
        });
        
        await _context.SaveChangesAsync();
        return GenerateToken(person);
    }

    private IResult GenerateToken(PersonInput person)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, person.Email) };
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        var response = new
        {
            access_token = new JwtSecurityTokenHandler().WriteToken(jwt),
            username = person.Email
        };
        return Results.Json(response);
    }
    
    private string HashString(string str, string saltString)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(str);
        byte[] salt = Convert.FromBase64String(saltString);
        byte[] saltedPassword = new byte[salt.Length + passwordBytes.Length];
        Array.Copy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
        Array.Copy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);
        
        using var hash = SHA256.Create();
        
        return Convert.ToBase64String(hash.ComputeHash(saltedPassword));
    }
}

public record class PersonInput(string Email, string Password);