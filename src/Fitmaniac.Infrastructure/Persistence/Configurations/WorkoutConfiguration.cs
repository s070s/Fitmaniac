using Fitmaniac.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fitmaniac.Infrastructure.Persistence.Configurations;

public sealed class WorkoutConfiguration : IEntityTypeConfiguration<Workout>
{
    public void Configure(EntityTypeBuilder<Workout> builder)
    {
        builder.HasIndex(w => w.ScheduledDateTime);
        builder.HasIndex(w => w.TrainerId);
        builder.HasIndex(w => w.WeeklyProgramId);

        builder.HasMany(w => w.Clients)
            .WithMany(c => c.Workouts)
            .UsingEntity<ClientWorkout>(
                j => j.HasOne(cw => cw.Client).WithMany().HasForeignKey(cw => cw.ClientId),
                j => j.HasOne(cw => cw.Workout).WithMany().HasForeignKey(cw => cw.WorkoutId));

        builder.HasMany(w => w.WorkoutExercises)
            .WithOne(we => we.Workout)
            .HasForeignKey(we => we.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
