using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Auth;

namespace Fitmaniac.Web.Consumers;

public interface IAuthConsumer
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
    Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default);
    Task<bool> LogoutAsync(CancellationToken ct = default);
    Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto dto, CancellationToken ct = default);
    Task<bool> ResetPasswordAsync(ResetPasswordRequestDto dto, CancellationToken ct = default);
    Task<bool> ConfirmEmailAsync(ConfirmEmailRequestDto dto, CancellationToken ct = default);
    Task<bool> ChangePasswordAsync(ChangePasswordRequestDto dto, CancellationToken ct = default);
}

public sealed class AuthConsumer(HttpClient http) : ApiClientBase(http), IAuthConsumer
{
    public Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken ct = default) =>
        PostAsync<AuthResponseDto>("/api/auth/login", dto, ct);

    public Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default) =>
        PostAsync<AuthResponseDto>("/api/auth/register", dto, ct);

    public Task<bool> LogoutAsync(CancellationToken ct = default) =>
        PostVoidAsync("/api/auth/logout", null, ct);

    public Task<bool> ForgotPasswordAsync(ForgotPasswordRequestDto dto, CancellationToken ct = default) =>
        PostVoidAsync("/api/auth/forgot-password", dto, ct);

    public Task<bool> ResetPasswordAsync(ResetPasswordRequestDto dto, CancellationToken ct = default) =>
        PostVoidAsync("/api/auth/reset-password", dto, ct);

    public Task<bool> ConfirmEmailAsync(ConfirmEmailRequestDto dto, CancellationToken ct = default) =>
        PostVoidAsync("/api/auth/confirm-email", dto, ct);

    public Task<bool> ChangePasswordAsync(ChangePasswordRequestDto dto, CancellationToken ct = default) =>
        PostVoidAsync("/api/auth/change-password", dto, ct);
}
