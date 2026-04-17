using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.Constants;
using Fitmaniac.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly IRefreshTokenCookieService _cookieService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext db,
        ITokenService tokenService,
        IEmailService emailService,
        IRefreshTokenCookieService cookieService)
    {
        _userManager = userManager;
        _db = db;
        _tokenService = tokenService;
        _emailService = emailService;
        _cookieService = cookieService;
    }

    public async Task<ServiceResult<object>> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing is not null)
            return ServiceResult<object>.Conflict("Email is already registered.");

        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            Role = UserRole.Client,
            Status = UserStatus.Pending,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return ServiceResult<object>.BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, RoleNames.Client);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        await _emailService.SendEmailConfirmationAsync(dto.Email, token, ct);

        return ServiceResult<object>.Ok(new { message = "Registration successful. Please confirm your email." });
    }

    public async Task<ServiceResult<object>> LoginAsync(LoginRequestDto dto, HttpContext ctx, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.UsernameOrEmail)
            ?? await _userManager.FindByNameAsync(dto.UsernameOrEmail);
        if (user is null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            return ServiceResult<object>.Unauthorized("Invalid email or password.");

        if (!user.EmailConfirmed)
            return ServiceResult<object>.Unauthorized("Email not confirmed.");

        if (!user.IsEnabled)
            return ServiceResult<object>.Forbidden("Account is disabled.");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var rawRefresh = _tokenService.GenerateRefreshToken();
        var tokenHash = _tokenService.HashToken(rawRefresh);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = tokenHash,
            ExpiresUtc = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ctx.Connection.RemoteIpAddress?.ToString(),
        };
        _db.RefreshTokens.Add(refreshToken);

        user.LastLoginUtc = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
        await _db.SaveChangesAsync(ct);

        var isMobile = ctx.Request.Headers[AuthConstants.MobileClientHeader] == AuthConstants.MobileClientValue;
        var refreshExpiry = DateTime.UtcNow.AddDays(7);
        if (isMobile)
            return ServiceResult<object>.Ok(new AuthResponseDto(true, accessToken, DateTime.UtcNow.AddMinutes(30), null));

        _cookieService.SetRefreshTokenCookie(ctx, rawRefresh, refreshExpiry);
        return ServiceResult<object>.Ok(new AccessTokenDto(accessToken, DateTime.UtcNow.AddMinutes(30)));
    }

    public async Task<ServiceResult<object>> RefreshAsync(HttpContext ctx, CancellationToken ct = default)
    {
        var isMobile = ctx.Request.Headers[AuthConstants.MobileClientHeader] == AuthConstants.MobileClientValue;
        string? rawToken;

        if (isMobile)
        {
            // Expect token in JSON body
            var body = await new System.IO.StreamReader(ctx.Request.Body).ReadToEndAsync(ct);
            var parsed = System.Text.Json.JsonSerializer.Deserialize<RefreshResponseDto>(body);
            rawToken = parsed?.RefreshToken;
        }
        else
        {
            rawToken = _cookieService.GetRefreshTokenFromCookie(ctx);
        }

        if (string.IsNullOrEmpty(rawToken))
            return ServiceResult<object>.Unauthorized("Refresh token missing.");

        var hash = _tokenService.HashToken(rawToken);
        var stored = await _db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.TokenHash == hash, ct);

        if (stored is null || !stored.IsActive)
            return ServiceResult<object>.Unauthorized("Invalid or expired refresh token.");

        var newRaw = _tokenService.GenerateRefreshToken();
        var newHash = _tokenService.HashToken(newRaw);

        stored.RevokedUtc = DateTime.UtcNow;
        stored.ReplacedByTokenHash = newHash;
        stored.RevokedByIp = ctx.Connection.RemoteIpAddress?.ToString();

        var newToken = new RefreshToken
        {
            UserId = stored.UserId,
            TokenHash = newHash,
            ExpiresUtc = DateTime.UtcNow.AddDays(7),
            CreatedByIp = ctx.Connection.RemoteIpAddress?.ToString(),
        };
        _db.RefreshTokens.Add(newToken);
        await _db.SaveChangesAsync(ct);

        var roles = await _userManager.GetRolesAsync(stored.User!);
        var accessToken = _tokenService.GenerateAccessToken(stored.User!, roles);

        if (isMobile)
            return ServiceResult<object>.Ok(new AuthResponseDto(true, accessToken, DateTime.UtcNow.AddMinutes(30), null));

        var newExpiry = DateTime.UtcNow.AddDays(7);
        _cookieService.SetRefreshTokenCookie(ctx, newRaw, newExpiry);
        return ServiceResult<object>.Ok(new AccessTokenDto(accessToken, DateTime.UtcNow.AddMinutes(30)));
    }

    public async Task<ServiceResult<object>> LogoutAsync(HttpContext ctx, CancellationToken ct = default)
    {
        var rawToken = _cookieService.GetRefreshTokenFromCookie(ctx);
        if (!string.IsNullOrEmpty(rawToken))
        {
            var hash = _tokenService.HashToken(rawToken);
            var stored = await _db.RefreshTokens.FirstOrDefaultAsync(r => r.TokenHash == hash, ct);
            if (stored is not null)
            {
                stored.RevokedUtc = DateTime.UtcNow;
                stored.RevokedByIp = ctx.Connection.RemoteIpAddress?.ToString();
                await _db.SaveChangesAsync(ct);
            }
        }
        _cookieService.ClearRefreshTokenCookie(ctx);
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<object>> ForgotPasswordAsync(ForgotPasswordRequestDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is not null)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _emailService.SendPasswordResetAsync(dto.Email, token, ct);
        }
        // Always return OK to prevent email enumeration
        return ServiceResult<object>.Ok(new { message = "If this email is registered, a reset link has been sent." });
    }

    public async Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordRequestDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return ServiceResult<object>.BadRequest("Invalid request.");

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded)
            return ServiceResult<object>.BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));

        return ServiceResult<object>.Ok(new { message = "Password reset successful." });
    }

    public async Task<ServiceResult<object>> ConfirmEmailAsync(ConfirmEmailRequestDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return ServiceResult<object>.NotFound("User not found.");

        var result = await _userManager.ConfirmEmailAsync(user, dto.Token);
        if (!result.Succeeded)
            return ServiceResult<object>.BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));

        user.Status = UserStatus.Active;
        await _userManager.UpdateAsync(user);
        return ServiceResult<object>.Ok(new { message = "Email confirmed." });
    }

    public async Task<ServiceResult<object>> ChangePasswordAsync(ChangePasswordRequestDto dto, int userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null)
            return ServiceResult<object>.NotFound("User not found.");

        var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
            return ServiceResult<object>.BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));

        return ServiceResult<object>.Ok(new { message = "Password changed." });
    }
}
