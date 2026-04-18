using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Workouts;

namespace Fitmaniac.Web.Consumers;

public interface IWorkoutConsumer
{
    Task<IReadOnlyList<WorkoutListItemDto>?> GetMyWorkoutsAsync(DateTime? from = null, DateTime? to = null, int? programId = null, CancellationToken ct = default);
    Task<WorkoutDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<bool> CompleteWorkoutAsync(int id, int? perceivedIntensity = null, string? notes = null, CancellationToken ct = default);
    Task<PagedResult<WorkoutListItemDto>?> GetAllWorkoutsAsync(int page = 1, int pageSize = 20, int? trainerId = null, CancellationToken ct = default);
    Task<WorkoutDto?> CreateWorkoutAsync(CreateWorkoutDto dto, CancellationToken ct = default);
    Task<WorkoutDto?> UpdateWorkoutAsync(int id, UpdateWorkoutDto dto, CancellationToken ct = default);
    Task<bool> DeleteWorkoutAsync(int id, CancellationToken ct = default);
}

public sealed class WorkoutConsumer(HttpClient http) : ApiClientBase(http), IWorkoutConsumer
{
    public Task<IReadOnlyList<WorkoutListItemDto>?> GetMyWorkoutsAsync(DateTime? from = null, DateTime? to = null, int? programId = null, CancellationToken ct = default) =>
        GetAsync<IReadOnlyList<WorkoutListItemDto>>(BuildUrl("/api/workouts", ("from", from), ("to", to), ("programId", programId)), ct);

    public Task<WorkoutDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        GetAsync<WorkoutDto>($"/api/workouts/{id}", ct);

    public Task<bool> CompleteWorkoutAsync(int id, int? perceivedIntensity = null, string? notes = null, CancellationToken ct = default) =>
        PostVoidAsync($"/api/workouts/{id}/complete?perceivedIntensity={perceivedIntensity}&notes={Uri.EscapeDataString(notes ?? "")}", null, ct);

    public Task<PagedResult<WorkoutListItemDto>?> GetAllWorkoutsAsync(int page = 1, int pageSize = 20, int? trainerId = null, CancellationToken ct = default) =>
        GetAsync<PagedResult<WorkoutListItemDto>>(BuildUrl("/api/admin/workouts", ("page", page), ("pageSize", pageSize), ("trainerId", trainerId)), ct);

    public Task<WorkoutDto?> CreateWorkoutAsync(CreateWorkoutDto dto, CancellationToken ct = default) =>
        PostAsync<WorkoutDto>("/api/admin/workouts", dto, ct);

    public Task<WorkoutDto?> UpdateWorkoutAsync(int id, UpdateWorkoutDto dto, CancellationToken ct = default) =>
        PutAsync<WorkoutDto>($"/api/admin/workouts/{id}", dto, ct);

    public Task<bool> DeleteWorkoutAsync(int id, CancellationToken ct = default) =>
        DeleteAsync($"/api/admin/workouts/{id}", ct);
}
