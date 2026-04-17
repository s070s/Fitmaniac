using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class Client : PersonalInformation
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    [StringLength(500)] public string? Bio { get; set; }
    public ClientExperience ExperienceLevel { get; set; } = ClientExperience.Beginner;
    public IntensityLevel PreferredIntensityLevel { get; set; } = IntensityLevel.Medium;

    public MedicalHistory? MedicalHistory { get; set; }
    public WeeklyProgram? CurrentWeeklyProgram { get; set; }

    public ICollection<Goal> Goals { get; set; } = new List<Goal>();
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
    public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
}
