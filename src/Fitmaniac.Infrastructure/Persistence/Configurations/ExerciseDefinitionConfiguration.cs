using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Fitmaniac.Infrastructure.Persistence.Configurations;

public sealed class ExerciseDefinitionConfiguration : IEntityTypeConfiguration<ExerciseDefinition>
{
    public void Configure(EntityTypeBuilder<ExerciseDefinition> builder)
    {
        builder.Property(e => e.PrimaryMuscleGroups)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<ICollection<MuscleGroup>>(v, (JsonSerializerOptions?)null) ?? new List<MuscleGroup>());

        builder.Property(e => e.SecondaryMuscleGroups)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<ICollection<MuscleGroup>>(v, (JsonSerializerOptions?)null) ?? new List<MuscleGroup>());

        builder.HasMany(e => e.Equipments)
            .WithMany(eq => eq.Exercises)
            .UsingEntity<ExerciseEquipment>(
                j => j.HasOne(ee => ee.Equipment).WithMany().HasForeignKey(ee => ee.EquipmentId),
                j => j.HasOne(ee => ee.ExerciseDefinition).WithMany().HasForeignKey(ee => ee.ExerciseDefinitionId));
    }
}
