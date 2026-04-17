using Fitmaniac.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fitmaniac.Infrastructure.Persistence.Configurations;

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasIndex(c => c.UserId).IsUnique();

        builder.HasOne(c => c.MedicalHistory)
            .WithOne(m => m.Client)
            .HasForeignKey<MedicalHistory>(m => m.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.CurrentWeeklyProgram)
            .WithOne(wp => wp.Client)
            .HasForeignKey<WeeklyProgram>(wp => wp.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Goals)
            .WithOne(g => g.Client)
            .HasForeignKey(g => g.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Measurements)
            .WithOne(m => m.Client)
            .HasForeignKey(m => m.ClientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
