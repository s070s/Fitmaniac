using Fitmaniac.Application.Abstractions;
using Fitmaniac.Shared.Constants;
using Microsoft.AspNetCore.Http;

namespace Fitmaniac.Infrastructure.Identity;

public sealed class RefreshTokenCookieService : IRefreshTokenCookieService
{
    private static readonly CookieOptions WebCookieOptions = new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/",
        MaxAge = TimeSpan.FromDays(7),
    };

    public void SetRefreshTokenCookie(HttpContext context, string token, DateTime expiresUtc)
    {
        var opts = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            Expires = expiresUtc,
        };
        context.Response.Cookies.Append(AuthConstants.RefreshCookieName, token, opts);
    }

    public string? GetRefreshTokenFromCookie(HttpContext context)
    {
        return context.Request.Cookies.TryGetValue(AuthConstants.RefreshCookieName, out var v) ? v : null;
    }

    public void ClearRefreshTokenCookie(HttpContext context)
    {
        context.Response.Cookies.Delete(AuthConstants.RefreshCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/",
        });
    }
}
