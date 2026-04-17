using Fitmaniac.Shared.DTOs.Clients;

namespace Fitmaniac.Web.Consumers;

public interface IClientConsumer
{
    Task<IReadOnlyList<int>?> GetSubscriptionsAsync(CancellationToken ct = default);
    Task<ClientDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> SubscribeAsync(int trainerId, CancellationToken ct = default);
    Task<bool> UnsubscribeAsync(int trainerId, CancellationToken ct = default);
}

public sealed class ClientConsumer(HttpClient http) : ApiClientBase(http), IClientConsumer
{
    public Task<IReadOnlyList<int>?> GetSubscriptionsAsync(CancellationToken ct = default) =>
        GetAsync<IReadOnlyList<int>>("/api/clients/me/subscriptions", ct);

    public Task<ClientDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        GetAsync<ClientDto>($"/api/clients/{id}", ct);

    public Task<bool> SubscribeAsync(int trainerId, CancellationToken ct = default) =>
        PostVoidAsync($"/api/clients/me/subscribe/{trainerId}", null, ct);

    public Task<bool> UnsubscribeAsync(int trainerId, CancellationToken ct = default) =>
        DeleteAsync($"/api/clients/me/subscribe/{trainerId}", ct);
}
