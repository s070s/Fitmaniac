using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Workouts;

namespace Fitmaniac.Application.Services;

public interface IWorkoutService
{
    Task<ServiceResult<IReadOnlyList<WorkoutListItemDto>>> GetMyWorkoutsAsync(int userId, DateTime? from, DateTime? to, int? programId, CancellationToken ct = default);
    Task<ServiceResult<WorkoutDto>> GetByIdAsync(int id, int userId, CancellationToken ct = default);
    Task<ServiceResult<object>> CompleteWorkoutAsync(int workoutId, int clientUserId, int? perceivedIntensity, string? notes, CancellationToken ct = default);
}
