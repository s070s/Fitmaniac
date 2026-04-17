using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Workouts;

namespace Fitmaniac.Application.Services;

public interface IAdminWorkoutService
{
    Task<ServiceResult<PagedResult<WorkoutListItemDto>>> GetWorkoutsAsync(int page, int pageSize, int? trainerId, CancellationToken ct = default);
    Task<ServiceResult<WorkoutDto>> CreateAsync(CreateWorkoutDto dto, CancellationToken ct = default);
    Task<ServiceResult<WorkoutDto>> UpdateAsync(int id, UpdateWorkoutDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<WorkoutExerciseDto>> AddExerciseAsync(CreateWorkoutExerciseDto dto, CancellationToken ct = default);
    Task<ServiceResult<WorkoutExerciseDto>> UpdateExerciseAsync(UpdateWorkoutExerciseDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> RemoveExerciseAsync(int workoutExerciseId, CancellationToken ct = default);
    Task<ServiceResult<WorkoutExerciseSetDto>> AddSetAsync(CreateWorkoutExerciseSetDto dto, CancellationToken ct = default);
    Task<ServiceResult<WorkoutExerciseSetDto>> UpdateSetAsync(UpdateWorkoutExerciseSetDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteSetAsync(int setId, CancellationToken ct = default);
}
