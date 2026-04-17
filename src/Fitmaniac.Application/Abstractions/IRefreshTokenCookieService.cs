using Microsoft.AspNetCore.Http;

namespace Fitmaniac.Application.Abstractions;

public interface IRefreshTokenCookieService
{
    void SetRefreshTokenCookie(HttpContext ctx, string token, DateTime expiresUtc);
    string? GetRefreshTokenFromCookie(HttpContext ctx);
    void ClearRefreshTokenCookie(HttpContext ctx);
}
