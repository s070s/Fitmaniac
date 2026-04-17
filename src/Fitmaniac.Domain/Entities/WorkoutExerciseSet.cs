using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class WorkoutExerciseSet : AuditableEntity
{
    public int WorkoutExerciseId { get; set; }
    public WorkoutExercise WorkoutExercise { get; set; } = null!;

    [Required, Range(1, 20)] public int SetNumber { get; set; }
    [Required, Range(1, 200)] public int Repetitions { get; set; }
    [Range(0, 1000)] public double? Weight { get; set; }
    public GoalUnit? GoalUnit { get; set; }
    [Required] public IntensityLevel OverallIntensityLevel { get; set; }
    [Range(0, int.MaxValue)] public int DurationInSeconds { get; set; }
    [Range(0, int.MaxValue)] public int RestPeriodInSeconds { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }
}
