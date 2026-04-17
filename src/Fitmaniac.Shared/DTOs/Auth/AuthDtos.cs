namespace Fitmaniac.Shared.DTOs.Auth;

public sealed record LoginRequestDto(string UsernameOrEmail, string Password);

public sealed record RegisterRequestDto(string Username, string Email, string Password, string Role);

public sealed record AuthResponseDto(bool Success, string? AccessToken, DateTime? AccessTokenExpiresUtc, string[]? Errors);

public sealed record AccessTokenDto(string AccessToken, DateTime AccessTokenExpiresUtc);

public sealed record RefreshResponseDto(
    string AccessToken,
    DateTime AccessTokenExpiresUtc,
    string? RefreshToken,
    DateTime? RefreshTokenExpiresUtc);

public sealed record ForgotPasswordRequestDto(string Email);

public sealed record ResetPasswordRequestDto(string Email, string Token, string NewPassword);

public sealed record ConfirmEmailRequestDto(string Email, string Token);

public sealed record ChangePasswordRequestDto(string CurrentPassword, string NewPassword);
