using Fitmaniac.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fitmaniac.Infrastructure.Persistence.Configurations;

public sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.Property(p => p.Price).HasColumnType("decimal(10,2)");
        builder.HasIndex(p => p.SubscriptionTier);
        builder.HasIndex(p => p.IsActive);
    }
}
