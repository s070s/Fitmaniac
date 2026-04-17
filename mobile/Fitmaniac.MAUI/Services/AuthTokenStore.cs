namespace Fitmaniac.MAUI.Services;

public interface IAuthTokenStore
{
    Task<string?> GetAccessTokenAsync();
    Task SetAccessTokenAsync(string token);
    Task ClearAsync();
}

public sealed class AuthTokenStore : IAuthTokenStore
{
    private const string Key = "fitmaniac_access_token";

    public Task<string?> GetAccessTokenAsync() =>
        Task.FromResult(SecureStorage.Default.GetAsync(Key).GetAwaiter().GetResult());

    public Task SetAccessTokenAsync(string token)
    {
        SecureStorage.Default.SetAsync(Key, token);
        return Task.CompletedTask;
    }

    public Task ClearAsync()
    {
        SecureStorage.Default.Remove(Key);
        return Task.CompletedTask;
    }
}
