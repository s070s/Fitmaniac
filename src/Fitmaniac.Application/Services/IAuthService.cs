using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Http;

namespace Fitmaniac.Application.Services;

public interface IAuthService
{
    Task<ServiceResult<object>> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> LoginAsync(LoginRequestDto dto, HttpContext ctx, CancellationToken ct = default);
    Task<ServiceResult<object>> RefreshAsync(HttpContext ctx, CancellationToken ct = default);
    Task<ServiceResult<object>> LogoutAsync(HttpContext ctx, CancellationToken ct = default);
    Task<ServiceResult<object>> ForgotPasswordAsync(ForgotPasswordRequestDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> ResetPasswordAsync(ResetPasswordRequestDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> ConfirmEmailAsync(ConfirmEmailRequestDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> ChangePasswordAsync(ChangePasswordRequestDto dto, int userId, CancellationToken ct = default);
}
