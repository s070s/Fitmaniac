using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Subscriptions;

namespace Fitmaniac.Application.Services;

public interface ISubscriptionService
{
    Task<ServiceResult<IReadOnlyList<SubscriptionPlanDto>>> GetPlansAsync(CancellationToken ct = default);
    Task<ServiceResult<UserSubscriptionDto?>> GetCurrentSubscriptionAsync(int userId, CancellationToken ct = default);
    Task<ServiceResult<object>> UpgradeAsync(int userId, int planId, CancellationToken ct = default);
}
