namespace Fitmaniac.MAUI.Services;

public interface IWorkoutService
{
    Task<PagedResultDto<WorkoutListItemDto>?> GetMyWorkoutsAsync(CancellationToken ct = default);
    Task<WorkoutDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> CompleteWorkoutAsync(int id, int? perceivedIntensity = null, string? notes = null, CancellationToken ct = default);
}

public sealed class WorkoutService(IApiClient api) : IWorkoutService
{
    public Task<PagedResultDto<WorkoutListItemDto>?> GetMyWorkoutsAsync(CancellationToken ct = default) =>
        api.GetAsync<PagedResultDto<WorkoutListItemDto>>("/api/workouts/me", ct);

    public Task<WorkoutDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        api.GetAsync<WorkoutDto>($"/api/workouts/{id}", ct);

    public Task<bool> CompleteWorkoutAsync(int id, int? perceivedIntensity = null, string? notes = null, CancellationToken ct = default) =>
        api.PostVoidAsync($"/api/workouts/{id}/complete?perceivedIntensity={perceivedIntensity}&notes={Uri.EscapeDataString(notes ?? "")}", null, ct);
}
