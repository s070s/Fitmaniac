using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Subscriptions;

namespace Fitmaniac.Application.Services;

public interface IBillingService
{
    Task<ServiceResult<PagedResult<BillingTransactionDto>>> GetTransactionsAsync(int userId, int page, int pageSize, CancellationToken ct = default);
}
