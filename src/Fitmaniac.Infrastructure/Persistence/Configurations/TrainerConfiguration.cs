using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Fitmaniac.Infrastructure.Persistence.Configurations;

public sealed class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
{
    public void Configure(EntityTypeBuilder<Trainer> builder)
    {
        builder.HasIndex(t => t.UserId).IsUnique();

        builder.HasMany(t => t.Clients)
            .WithMany(c => c.Trainers)
            .UsingEntity<TrainerClient>(
                j => j.HasOne(tc => tc.Client).WithMany().HasForeignKey(tc => tc.ClientId),
                j => j.HasOne(tc => tc.Trainer).WithMany().HasForeignKey(tc => tc.TrainerId));

        builder.HasMany(t => t.Workouts)
            .WithOne(w => w.Trainer)
            .HasForeignKey(w => w.TrainerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(t => t.Specializations)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<ICollection<TrainerSpecialization>>(v, (JsonSerializerOptions?)null) ?? new List<TrainerSpecialization>());
    }
}
