using Fitmaniac.Domain.Entities;

namespace Fitmaniac.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    string HashToken(string token);
    int GetUserId(string accessToken);
}
