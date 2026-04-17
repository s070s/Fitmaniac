using Fitmaniac.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fitmaniac.Infrastructure.Persistence.Configurations;

public sealed class WorkoutExerciseConfiguration : IEntityTypeConfiguration<WorkoutExercise>
{
    public void Configure(EntityTypeBuilder<WorkoutExercise> builder)
    {
        builder.HasMany(we => we.Sets)
            .WithOne(s => s.WorkoutExercise)
            .HasForeignKey(s => s.WorkoutExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(we => we.ExerciseDefinition)
            .WithMany(ed => ed.WorkoutExercises)
            .HasForeignKey(we => we.ExerciseDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
