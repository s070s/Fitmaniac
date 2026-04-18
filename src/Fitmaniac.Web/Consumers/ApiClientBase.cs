using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace Fitmaniac.Web.Consumers;

public abstract class ApiClientBase(HttpClient http)
{
    protected HttpClient Http { get; } = http;

    protected static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web);

    protected static string BuildUrl(string path, params (string Key, object? Value)[] query)
    {
        var builder = HttpUtility.ParseQueryString(string.Empty);

        foreach (var (key, value) in query)
        {
            if (value is null)
            {
                continue;
            }

            builder[key] = value switch
            {
                DateTime dateTime => dateTime.ToString("O"),
                DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O"),
                _ => value.ToString()
            };
        }

        var queryString = builder.ToString();
        return string.IsNullOrEmpty(queryString) ? path : $"{path}?{queryString}";
    }

    protected async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        var response = await Http.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<T>(JsonOpts, ct);
    }

    protected async Task<T?> PostAsync<T>(string url, object? body = null, CancellationToken ct = default)
    {
        var response = body is null ? await Http.PostAsync(url, null, ct) : await Http.PostAsJsonAsync(url, body, JsonOpts, ct);
        if (!response.IsSuccessStatusCode) return default;
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return default;
        return await response.Content.ReadFromJsonAsync<T>(JsonOpts, ct);
    }

    protected async Task<bool> PostVoidAsync(string url, object? body = null, CancellationToken ct = default)
    {
        var response = body is null ? await Http.PostAsync(url, null, ct) : await Http.PostAsJsonAsync(url, body, JsonOpts, ct);
        return response.IsSuccessStatusCode;
    }

    protected async Task<T?> PutAsync<T>(string url, object body, CancellationToken ct = default)
    {
        var response = await Http.PutAsJsonAsync(url, body, JsonOpts, ct);
        if (!response.IsSuccessStatusCode) return default;
        return await response.Content.ReadFromJsonAsync<T>(JsonOpts, ct);
    }

    protected async Task<bool> DeleteAsync(string url, CancellationToken ct = default)
    {
        var response = await Http.DeleteAsync(url, ct);
        return response.IsSuccessStatusCode;
    }
}
