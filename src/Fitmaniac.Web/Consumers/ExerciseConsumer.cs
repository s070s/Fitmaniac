using Fitmaniac.Application.Common;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Exercises;

namespace Fitmaniac.Web.Consumers;

public interface IExerciseConsumer
{
    Task<PagedResult<ExerciseDefinitionDto>?> GetExercisesAsync(int page = 1, int pageSize = 20, string? category = null, MuscleGroup? muscleGroup = null, ClientExperience? experience = null, int? equipmentId = null, CancellationToken ct = default);
    Task<ExerciseDefinitionDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<EquipmentDto>?> GetEquipmentAsync(CancellationToken ct = default);
}

public sealed class ExerciseConsumer(HttpClient http) : ApiClientBase(http), IExerciseConsumer
{
    public Task<PagedResult<ExerciseDefinitionDto>?> GetExercisesAsync(int page = 1, int pageSize = 20, string? category = null, MuscleGroup? muscleGroup = null, ClientExperience? experience = null, int? equipmentId = null, CancellationToken ct = default) =>
        GetAsync<PagedResult<ExerciseDefinitionDto>>($"/api/exercises?page={page}&pageSize={pageSize}&category={category}&muscleGroup={muscleGroup}&experience={experience}&equipmentId={equipmentId}", ct);

    public Task<ExerciseDefinitionDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        GetAsync<ExerciseDefinitionDto>($"/api/exercises/{id}", ct);

    public Task<IReadOnlyList<EquipmentDto>?> GetEquipmentAsync(CancellationToken ct = default) =>
        GetAsync<IReadOnlyList<EquipmentDto>>("/api/equipment", ct);
}
