using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class WeeklyProgram : AuditableEntity
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }
    [Required, Range(1, 52)] public int DurationInWeeks { get; set; }
    [Required, Range(1, 52)] public int CurrentWeek { get; set; } = 1;

    [NotMapped] public bool IsCompleted => CurrentWeek > DurationInWeeks;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
