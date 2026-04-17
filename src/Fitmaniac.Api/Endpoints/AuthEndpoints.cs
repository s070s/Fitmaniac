using Fitmaniac.Application.Services;
using Fitmaniac.Shared.DTOs.Auth;

namespace Fitmaniac.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/auth");
        g.MapPost("/register", async (RegisterRequestDto dto, IAuthService svc) =>
            (await svc.RegisterAsync(dto)).ToResult()).AllowAnonymous();
        g.MapPost("/login", async (LoginRequestDto dto, HttpContext ctx, IAuthService svc) =>
            (await svc.LoginAsync(dto, ctx)).ToResult()).AllowAnonymous().RequireRateLimiting("login");
        g.MapPost("/refresh", async (HttpContext ctx, IAuthService svc) =>
            (await svc.RefreshAsync(ctx)).ToResult()).AllowAnonymous().RequireRateLimiting("refresh");
        g.MapPost("/logout", async (HttpContext ctx, IAuthService svc) =>
            (await svc.LogoutAsync(ctx)).ToResult()).RequireAuthorization();
        g.MapPost("/forgot-password", async (ForgotPasswordRequestDto dto, IAuthService svc) =>
            (await svc.ForgotPasswordAsync(dto)).ToResult()).AllowAnonymous();
        g.MapPost("/reset-password", async (ResetPasswordRequestDto dto, IAuthService svc) =>
            (await svc.ResetPasswordAsync(dto)).ToResult()).AllowAnonymous();
        g.MapPost("/confirm-email", async (ConfirmEmailRequestDto dto, IAuthService svc) =>
            (await svc.ConfirmEmailAsync(dto)).ToResult()).AllowAnonymous();
        g.MapPost("/change-password", async (ChangePasswordRequestDto dto, HttpContext ctx, IAuthService svc) =>
        {
            var userId = int.Parse(ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            return (await svc.ChangePasswordAsync(dto, userId)).ToResult();
        }).RequireAuthorization();
        return e;
    }
}
