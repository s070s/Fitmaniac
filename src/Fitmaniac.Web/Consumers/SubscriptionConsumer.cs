using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Subscriptions;

namespace Fitmaniac.Web.Consumers;

public interface ISubscriptionConsumer
{
    Task<IReadOnlyList<SubscriptionPlanDto>?> GetPlansAsync(CancellationToken ct = default);
    Task<UserSubscriptionDto?> GetCurrentSubscriptionAsync(CancellationToken ct = default);
    Task<bool> UpgradeAsync(int planId, CancellationToken ct = default);
}

public interface IBillingConsumer
{
    Task<PagedResult<BillingTransactionDto>?> GetTransactionsAsync(int page = 1, int pageSize = 20, CancellationToken ct = default);
}

public sealed class SubscriptionConsumer(HttpClient http) : ApiClientBase(http), ISubscriptionConsumer
{
    public Task<IReadOnlyList<SubscriptionPlanDto>?> GetPlansAsync(CancellationToken ct = default) =>
        GetAsync<IReadOnlyList<SubscriptionPlanDto>>("/api/subscriptions/plans", ct);

    public Task<UserSubscriptionDto?> GetCurrentSubscriptionAsync(CancellationToken ct = default) =>
        GetAsync<UserSubscriptionDto>("/api/subscriptions/me", ct);

    public Task<bool> UpgradeAsync(int planId, CancellationToken ct = default) =>
        PostVoidAsync($"/api/subscriptions/upgrade/{planId}", null, ct);
}

public sealed class BillingConsumer(HttpClient http) : ApiClientBase(http), IBillingConsumer
{
    public Task<PagedResult<BillingTransactionDto>?> GetTransactionsAsync(int page = 1, int pageSize = 20, CancellationToken ct = default) =>
        GetAsync<PagedResult<BillingTransactionDto>>($"/api/billing/transactions?page={page}&pageSize={pageSize}", ct);
}
