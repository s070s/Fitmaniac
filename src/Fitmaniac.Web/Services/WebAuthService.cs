using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fitmaniac.Shared.DTOs.Auth;

namespace Fitmaniac.Web.Services;

public sealed class WebAuthService(IHttpContextAccessor httpContextAccessor)
{
    public async Task<bool> SignInAsync(AuthResponseDto response)
    {
        var ctx = httpContextAccessor.HttpContext;
        if (ctx is null || string.IsNullOrEmpty(response.AccessToken)) return false;

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken? jwt = null;
        try { jwt = handler.ReadJwtToken(response.AccessToken); } catch { return false; }

        var claims = jwt.Claims.ToList();
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var props = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
        };
        props.StoreTokens([new AuthenticationToken { Name = "access_token", Value = response.AccessToken }]);

        await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
        return true;
    }

    public async Task SignOutAsync()
    {
        var ctx = httpContextAccessor.HttpContext;
        if (ctx is not null)
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}
