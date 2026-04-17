using Fitmaniac.Application.Common;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Exercises;

namespace Fitmaniac.Application.Services;

public interface IExerciseDefinitionService
{
    Task<ServiceResult<PagedResult<ExerciseDefinitionDto>>> GetExercisesAsync(int page, int pageSize, string? category, MuscleGroup? muscleGroup, ClientExperience? experience, int? equipmentId, CancellationToken ct = default);
    Task<ServiceResult<ExerciseDefinitionDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<ExerciseDefinitionDto>> CreateAsync(CreateExerciseDefinitionDto dto, CancellationToken ct = default);
    Task<ServiceResult<ExerciseDefinitionDto>> UpdateAsync(UpdateExerciseDefinitionDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default);
}
