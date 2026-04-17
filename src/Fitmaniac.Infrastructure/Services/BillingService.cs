using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Shared.DTOs.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class BillingService : IBillingService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public BillingService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PagedResult<BillingTransactionDto>>> GetTransactionsAsync(int userId, int page, int pageSize, CancellationToken ct = default)
    {
        var query = _db.BillingTransactions.Where(t => t.UserId == userId).OrderByDescending(t => t.CreatedUtc);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return ServiceResult<PagedResult<BillingTransactionDto>>.Ok(
            new PagedResult<BillingTransactionDto>(items.Select(t => _mapper.ToDto(t)!).ToList(), total, page, pageSize));
    }
}
