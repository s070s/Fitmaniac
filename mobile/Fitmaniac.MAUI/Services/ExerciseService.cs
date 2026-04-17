namespace Fitmaniac.MAUI.Services;

public interface IExerciseService
{
    Task<PagedResultDto<ExerciseDefinitionDto>?> GetExercisesAsync(CancellationToken ct = default);
    Task<ExerciseDefinitionDto?> GetByIdAsync(int id, CancellationToken ct = default);
}

public sealed class ExerciseService(IApiClient api) : IExerciseService
{
    public Task<PagedResultDto<ExerciseDefinitionDto>?> GetExercisesAsync(CancellationToken ct = default) =>
        api.GetAsync<PagedResultDto<ExerciseDefinitionDto>>("/api/exercises", ct);

    public Task<ExerciseDefinitionDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        api.GetAsync<ExerciseDefinitionDto>($"/api/exercises/{id}", ct);
}
