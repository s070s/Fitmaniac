using Fitmaniac.Shared.DTOs.Goals;

namespace Fitmaniac.MAUI.Services;

public interface IGoalService
{
    Task<IReadOnlyList<GoalDto>?> GetMyGoalsAsync(CancellationToken ct = default);
    Task<GoalDto?> CreateAsync(CreateGoalDto dto, CancellationToken ct = default);
}

public sealed class GoalService(IApiClient api) : IGoalService
{
    public Task<IReadOnlyList<GoalDto>?> GetMyGoalsAsync(CancellationToken ct = default) =>
        api.GetAsync<IReadOnlyList<GoalDto>>("/api/goals/me", ct);

    public Task<GoalDto?> CreateAsync(CreateGoalDto dto, CancellationToken ct = default) =>
        api.PostAsync<GoalDto>("/api/goals", dto, ct);
}
