using Fitmaniac.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fitmaniac.Infrastructure.Persistence.Configurations;

public sealed class TrainerClientConfiguration : IEntityTypeConfiguration<TrainerClient>
{
    public void Configure(EntityTypeBuilder<TrainerClient> builder)
    {
        builder.HasKey(tc => new { tc.TrainerId, tc.ClientId });
        builder.HasIndex(tc => tc.SubscribedUtc);
    }
}
