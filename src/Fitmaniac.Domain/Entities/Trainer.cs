using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class Trainer : PersonalInformation
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    [StringLength(1000)] public string? Bio { get; set; }
    public ICollection<TrainerSpecialization> Specializations { get; set; } = new List<TrainerSpecialization>();

    public ICollection<Client> Clients { get; set; } = new List<Client>();
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
