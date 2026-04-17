using Fitmaniac.Application.Data;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fitmaniac.Infrastructure.Seeders;

public sealed class SubscriptionPlanSeeder
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<SubscriptionPlanSeeder> _logger;

    public SubscriptionPlanSeeder(IApplicationDbContext db, ILogger<SubscriptionPlanSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _db.SubscriptionPlans.AnyAsync()) return;

        _db.SubscriptionPlans.AddRange(
            new SubscriptionPlan { Name = "Free", Description = "Basic access", Price = 0m, BillingPeriod = BillingPeriod.Monthly, SubscriptionTier = SubscriptionTier.Free, IsActive = true },
            new SubscriptionPlan { Name = "Fitmaniac Plus (Monthly)", Description = "Full access monthly", Price = 9.99m, BillingPeriod = BillingPeriod.Monthly, SubscriptionTier = SubscriptionTier.Plus, IsActive = true },
            new SubscriptionPlan { Name = "Fitmaniac Plus (Yearly)", Description = "Full access yearly", Price = 89.99m, BillingPeriod = BillingPeriod.Yearly, SubscriptionTier = SubscriptionTier.Plus, IsActive = true },
            new SubscriptionPlan { Name = "Fitmaniac Pro (Monthly)", Description = "Pro features monthly", Price = 19.99m, BillingPeriod = BillingPeriod.Monthly, SubscriptionTier = SubscriptionTier.Pro, IsActive = true },
            new SubscriptionPlan { Name = "Fitmaniac Pro (Yearly)", Description = "Pro features yearly", Price = 179.99m, BillingPeriod = BillingPeriod.Yearly, SubscriptionTier = SubscriptionTier.Pro, IsActive = true }
        );

        await _db.SaveChangesAsync();
        _logger.LogInformation("Subscription plans seeded.");
    }
}
