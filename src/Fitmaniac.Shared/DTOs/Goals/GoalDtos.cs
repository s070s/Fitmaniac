namespace Fitmaniac.Shared.DTOs.Goals;

public sealed record GoalDto(
    int Id,
    int ClientId,
    GoalType GoalType,
    string? Description,
    DateTime TargetDate,
    GoalStatus Status,
    int? GoalQuantity,
    GoalUnit? GoalUnit);

public sealed record CreateGoalDto(
    int ClientId,
    GoalType GoalType,
    string? Description,
    DateTime TargetDate,
    int? GoalQuantity,
    GoalUnit? GoalUnit);

public sealed record UpdateGoalDto(
    int Id,
    GoalType? GoalType,
    string? Description,
    DateTime? TargetDate,
    GoalStatus? Status,
    int? GoalQuantity,
    GoalUnit? GoalUnit);
