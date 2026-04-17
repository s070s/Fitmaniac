using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class SubscriptionPlan : AuditableEntity
{
    [Required, StringLength(100)] public string Name { get; set; } = null!;
    [StringLength(500)] public string? Description { get; set; }
    [Required, Range(0, double.MaxValue)] public decimal Price { get; set; }
    [Required] public BillingPeriod BillingPeriod { get; set; }
    [Required] public SubscriptionTier SubscriptionTier { get; set; }
    public string? FeaturesJson { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
}
