using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class BillingTransaction : AuditableEntity
{
    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    public int? SubscriptionPlanId { get; set; }
    public SubscriptionPlan? SubscriptionPlan { get; set; }

    [Required, Range(0, double.MaxValue)] public decimal Amount { get; set; }
    [Required, StringLength(100)] public string TransactionReference { get; set; } = null!;
    [Required, StringLength(50)] public string Status { get; set; } = null!;
}
