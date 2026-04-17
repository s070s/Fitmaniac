using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class WorkoutExercise : AuditableEntity
{
    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;

    public int ExerciseDefinitionId { get; set; }
    public ExerciseDefinition ExerciseDefinition { get; set; } = null!;

    [MaxLength(500)] public string? Notes { get; set; }

    public ICollection<WorkoutExerciseSet> Sets { get; set; } = new List<WorkoutExerciseSet>();
}
