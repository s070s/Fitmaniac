namespace Fitmaniac.Shared.DTOs.Subscriptions;

public sealed record SubscriptionPlanDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    BillingPeriod BillingPeriod,
    SubscriptionTier SubscriptionTier,
    string? FeaturesJson,
    bool IsActive);

public sealed record UserSubscriptionDto(
    int Id,
    int UserId,
    int SubscriptionPlanId,
    string PlanName,
    SubscriptionTier Tier,
    DateTime StartDate,
    DateTime? EndDate,
    bool IsActive);

public sealed record BillingTransactionDto(
    int Id,
    int UserId,
    int? SubscriptionPlanId,
    string? PlanName,
    decimal Amount,
    string? TransactionReference,
    string Status,
    DateTime CreatedUtc);
