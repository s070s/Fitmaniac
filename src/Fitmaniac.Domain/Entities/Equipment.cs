using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class Equipment : AuditableEntity
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }

    public ICollection<ExerciseDefinition> Exercises { get; set; } = new List<ExerciseDefinition>();
}
