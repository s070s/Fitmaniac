using System.Net.Http.Json;

namespace Fitmaniac.MAUI.Services;

public interface IApiClient
{
    Task<T?> GetAsync<T>(string url, CancellationToken ct = default);
    Task<T?> PostAsync<T>(string url, object? body, CancellationToken ct = default);
    Task<bool> PostVoidAsync(string url, object? body, CancellationToken ct = default);
    Task<T?> PutAsync<T>(string url, object? body, CancellationToken ct = default);
    Task<bool> DeleteAsync(string url, CancellationToken ct = default);
}

public sealed class ApiClient(HttpClient http) : IApiClient
{
    public async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        var resp = await http.GetAsync(url, ct);
        if (!resp.IsSuccessStatusCode) return default;
        return await resp.Content.ReadFromJsonAsync<T>(ct);
    }

    public async Task<T?> PostAsync<T>(string url, object? body, CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync(url, body, ct);
        if (!resp.IsSuccessStatusCode) return default;
        return await resp.Content.ReadFromJsonAsync<T>(ct);
    }

    public async Task<bool> PostVoidAsync(string url, object? body, CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync(url, body, ct);
        return resp.IsSuccessStatusCode;
    }

    public async Task<T?> PutAsync<T>(string url, object? body, CancellationToken ct = default)
    {
        var resp = await http.PutAsJsonAsync(url, body, ct);
        if (!resp.IsSuccessStatusCode) return default;
        return await resp.Content.ReadFromJsonAsync<T>(ct);
    }

    public async Task<bool> DeleteAsync(string url, CancellationToken ct = default)
    {
        var resp = await http.DeleteAsync(url, ct);
        return resp.IsSuccessStatusCode;
    }
}
