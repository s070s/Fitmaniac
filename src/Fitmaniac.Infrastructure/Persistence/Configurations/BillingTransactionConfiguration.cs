using Fitmaniac.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fitmaniac.Infrastructure.Persistence.Configurations;

public sealed class BillingTransactionConfiguration : IEntityTypeConfiguration<BillingTransaction>
{
    public void Configure(EntityTypeBuilder<BillingTransaction> builder)
    {
        builder.Property(b => b.Amount).HasColumnType("decimal(10,2)");
        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => b.CreatedUtc);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<SubscriptionPlan>()
            .WithMany()
            .HasForeignKey(b => b.SubscriptionPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
