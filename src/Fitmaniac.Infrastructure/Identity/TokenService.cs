using Fitmaniac.Application.Abstractions;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Fitmaniac.Infrastructure.Identity;

public sealed class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = DateTime.UtcNow.AddMinutes(30);

        var claims = new List<Claim>
        {
            new(ClaimTypesExtended.UserId, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.UserName ?? user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiry,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }

    public int GetUserId(string accessToken)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(accessToken)) return 0;
        var jwt = handler.ReadJwtToken(accessToken);
        var claim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypesExtended.UserId)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }
}
