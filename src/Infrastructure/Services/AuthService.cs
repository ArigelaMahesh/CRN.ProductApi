using Application.DTOs;
using Application.DTOs.Interfaces;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        var existingUser = await _context.Users
            .AnyAsync(x => x.UserName == dto.UserName);

        if (existingUser)
            return false;

        var user = new User
        {
            UserName = dto.UserName,
            Role = "User"
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(user, dto.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.UserName == dto.UserName);

        if (user == null)
            return null;

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        return await GenerateTokensAsync(user);
    }

    public async Task<AuthResponseDto?> RefreshTokenAsync(
        string refreshToken)
    {
        var token = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.Token == refreshToken &&
                !x.IsRevoked);

        if (token == null || token.ExpiresOn <= DateTime.UtcNow)
            return null;

        token.IsRevoked = true;

        var response = await GenerateTokensAsync(token.User);

        await _context.SaveChangesAsync();

        return response;
    }

    private async Task<AuthResponseDto> GenerateTokensAsync(User user)
    {
        var accessMinutes = int.Parse(
    _configuration["Jwt:AccessTokenExpirationMinutes"]!);

        var refreshDays = int.Parse(
            _configuration["Jwt:RefreshTokenExpirationDays"]!);

        var accessExpiration = DateTime.UtcNow.AddMinutes(accessMinutes);

        var refreshExpiration = DateTime.UtcNow.AddDays(refreshDays);

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                user.UserName),

            new Claim(
                ClaimTypes.Role,
                user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: accessExpiration,
            signingCredentials: credentials);

        var accessToken =
            new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshExpiration,
            IsRevoked = false,
            UserId = user.Id
        });

        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = accessExpiration,
            RefreshTokenExpiresAt = refreshExpiration
        };
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(64));
    }
}