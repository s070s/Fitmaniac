using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class ExerciseDefinition : AuditableEntity
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    [StringLength(1000)] public string? Description { get; set; }
    [Url] public string? VideoUrl { get; set; }
    [Range(0, 2000)] public int CaloriesBurnedPerHour { get; set; }
    public bool IsCompoundExercise { get; set; }

    [Required, MinLength(1)]
    public ICollection<MuscleGroup> PrimaryMuscleGroups { get; set; } = new List<MuscleGroup>();
    public ICollection<MuscleGroup> SecondaryMuscleGroups { get; set; } = new List<MuscleGroup>();

    public ClientExperience? ExperienceLevel { get; set; }
    [StringLength(50)] public string? Category { get; set; }

    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
}
