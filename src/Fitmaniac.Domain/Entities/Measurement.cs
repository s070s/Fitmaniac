using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class Measurement : AuditableEntity
{
    [Required] public GoalUnit Unit { get; set; }
    [Required, Range(0, 1000)] public double Value { get; set; }
    [Required] public DateTime Date { get; set; } = DateTime.UtcNow;
    [Required] public IntensityLevel Intensity { get; set; }
    public bool IsPersonalBest { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}
