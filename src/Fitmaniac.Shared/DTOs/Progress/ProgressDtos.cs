namespace Fitmaniac.Shared.DTOs.Progress;

public sealed record ProgressSummaryDto
{
    public int TotalWorkouts { get; init; }
    public int CompletedWorkouts { get; init; }
    public int TotalGoals { get; init; }
    public int CompletedGoals { get; init; }
    public double TotalCaloriesBurned { get; init; }
    public int TotalPersonalBests { get; init; }
}

public sealed record WeeklyProgressDto
{
    public int WeekNumber { get; init; }
    public int Year { get; init; }
    public int CompletedWorkouts { get; init; }
    public double CaloriesBurned { get; init; }
}
