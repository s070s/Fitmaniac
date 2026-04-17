namespace Fitmaniac.MAUI.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken ct = default);
    Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default);
    Task<bool> LogoutAsync(CancellationToken ct = default);
    Task<AuthResponseDto?> RefreshAsync(CancellationToken ct = default);
}

public sealed class AuthService(IApiClient api, IAuthTokenStore tokenStore) : IAuthService
{
    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto, CancellationToken ct = default)
    {
        var result = await api.PostAsync<AuthResponseDto>("/api/auth/login", dto, ct);
        if (result is not null)
            await tokenStore.SetAccessTokenAsync(result.AccessToken);
        return result;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto, CancellationToken ct = default)
    {
        var result = await api.PostAsync<AuthResponseDto>("/api/auth/register", dto, ct);
        if (result is not null)
            await tokenStore.SetAccessTokenAsync(result.AccessToken);
        return result;
    }

    public async Task<bool> LogoutAsync(CancellationToken ct = default)
    {
        var ok = await api.PostVoidAsync("/api/auth/logout", null, ct);
        await tokenStore.ClearAsync();
        return ok;
    }

    public async Task<AuthResponseDto?> RefreshAsync(CancellationToken ct = default)
    {
        var result = await api.PostAsync<AuthResponseDto>("/api/auth/refresh", null, ct);
        if (result is not null)
            await tokenStore.SetAccessTokenAsync(result.AccessToken);
        return result;
    }
}
