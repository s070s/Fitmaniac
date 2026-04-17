using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class UserSubscription : AuditableEntity
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public int SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;

    [Required] public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
}
