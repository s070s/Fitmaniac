using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class Goal : AuditableEntity
{
    [Required] public GoalType GoalType { get; set; }
    [StringLength(255)] public string? Description { get; set; }
    [Required] public DateTime TargetDate { get; set; }
    [Required] public GoalStatus Status { get; set; } = GoalStatus.Active;
    [Range(0, 1000)] public int? GoalQuantity { get; set; }
    public GoalUnit? GoalUnit { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}
