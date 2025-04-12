using InventoryApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using InventoryApp.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryApp.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly ProductContext _context;

    public AuthenticationService(ProductContext context)
    {
        _context = context;
    }

    public async Task<(bool success, string message)> Register(string username, string password, string role, Guid? storeId)
    {
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            return (false, "Username already exists.");
        }

        if (role == "StoreAdmin")
        {
            if (storeId == null)
                return (false, "Store ID is required for store admins.");

            var store = await _context.Stores.AnyAsync(s => s.Id == storeId);
            if (!store)
                return (false, "The provided store ID does not exist.");
        }
        if (role == "CentralAdmin" && storeId != null)
            return (false, "Central admin should not be assigned to a store.");

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var user = new User
        {
            Username = username,
            PasswordHash = hashedPassword,
            Role = role,
            StoreId = storeId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return (true, "User registered successfully.");
    }


    public async Task<(bool success, string? token, string message)> Login(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, null, "Invalid credentials");

        var token = GenerateJwtToken(user);
        return (true, token, "Login successful");
    }

    public string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("StoreId", user.StoreId?.ToString() ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSuperSecretKey1234567890123456"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "InventoryApp",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
