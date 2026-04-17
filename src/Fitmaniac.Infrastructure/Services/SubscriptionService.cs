using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.Subscriptions;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class SubscriptionService : ISubscriptionService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public SubscriptionService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<SubscriptionPlanDto>>> GetPlansAsync(CancellationToken ct = default)
    {
        var plans = await _db.SubscriptionPlans.Where(p => p.IsActive).OrderBy(p => p.Price).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<SubscriptionPlanDto>>.Ok(plans.Select(p => _mapper.ToDto(p)!).ToList());
    }

    public async Task<ServiceResult<UserSubscriptionDto?>> GetCurrentSubscriptionAsync(int userId, CancellationToken ct = default)
    {
        var sub = await _db.UserSubscriptions
            .Include(s => s.SubscriptionPlan)
            .Where(s => s.UserId == userId && s.IsActive && s.EndDate >= DateTime.UtcNow)
            .OrderByDescending(s => s.StartDate)
            .FirstOrDefaultAsync(ct);

        return ServiceResult<UserSubscriptionDto?>.Ok(_mapper.ToDto(sub));
    }

    public async Task<ServiceResult<object>> UpgradeAsync(int userId, int planId, CancellationToken ct = default)
    {
        var plan = await _db.SubscriptionPlans.FindAsync([planId], ct);
        if (plan is null || !plan.IsActive) return ServiceResult<object>.NotFound("Plan not found.");

        // Deactivate existing
        var existing = await _db.UserSubscriptions.Where(s => s.UserId == userId && s.IsActive).ToListAsync(ct);
        foreach (var s in existing) s.IsActive = false;

        var newSub = new UserSubscription
        {
            UserId = userId,
            SubscriptionPlanId = planId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddMonths(1),
            IsActive = true,
        };
        _db.UserSubscriptions.Add(newSub);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.Created(new { message = "Subscribed." });
    }
}
