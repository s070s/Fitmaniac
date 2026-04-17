using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;

namespace Fitmaniac.Web.Services;

/// <summary>
/// Attaches the JWT access token (stored in the cookie auth ticket) to outgoing API requests.
/// </summary>
public sealed class SsrApiHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var ctx = httpContextAccessor.HttpContext;
        if (ctx is not null)
        {
            var token = await ctx.GetTokenAsync(CookieAuthenticationDefaults.AuthenticationScheme, "access_token");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, ct);
    }
}
