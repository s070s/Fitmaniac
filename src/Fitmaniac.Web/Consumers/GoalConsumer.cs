using Fitmaniac.Shared.DTOs.Goals;

namespace Fitmaniac.Web.Consumers;

public interface IGoalConsumer
{
    Task<IReadOnlyList<GoalDto>?> GetMyGoalsAsync(CancellationToken ct = default);
    Task<GoalDto?> CreateAsync(CreateGoalDto dto, CancellationToken ct = default);
    Task<GoalDto?> UpdateAsync(UpdateGoalDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public sealed class GoalConsumer(HttpClient http) : ApiClientBase(http), IGoalConsumer
{
    public Task<IReadOnlyList<GoalDto>?> GetMyGoalsAsync(CancellationToken ct = default) =>
        GetAsync<IReadOnlyList<GoalDto>>("/api/goals/me", ct);

    public Task<GoalDto?> CreateAsync(CreateGoalDto dto, CancellationToken ct = default) =>
        PostAsync<GoalDto>("/api/goals", dto, ct);

    public Task<GoalDto?> UpdateAsync(UpdateGoalDto dto, CancellationToken ct = default) =>
        PutAsync<GoalDto>("/api/goals", dto, ct);

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default) =>
        DeleteAsync($"/api/goals/{id}", ct);
}
