using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Extensions;
using Fitmaniac.Shared.DTOs.Workouts;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class WorkoutService : IWorkoutService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public WorkoutService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<WorkoutListItemDto>>> GetMyWorkoutsAsync(int userId, DateTime? from, DateTime? to, int? programId, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == userId, ct);
        if (client is null) return ServiceResult<IReadOnlyList<WorkoutListItemDto>>.NotFound("Client profile not found.");

        var query = _db.Workouts
            .Where(w => w.Clients.Any(c => c.UserId == userId))
            .InRange(from, to);

        if (programId.HasValue)
            query = query.Where(w => w.WeeklyProgramId == programId.Value);

        var items = await query.OrderByDescending(w => w.ScheduledDateTime).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<WorkoutListItemDto>>.Ok(items.Select(w => _mapper.ToListItemDto(w, userId)!).ToList());
    }

    public async Task<ServiceResult<WorkoutDto>> GetByIdAsync(int id, int userId, CancellationToken ct = default)
    {
        var workout = await _db.Workouts
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.Sets)
            .Include(w => w.WorkoutExercises)
                .ThenInclude(we => we.ExerciseDefinition)
            .FirstOrDefaultAsync(w => w.Id == id && w.Clients.Any(c => c.UserId == userId), ct);

        if (workout is null) return ServiceResult<WorkoutDto>.NotFound("Workout not found.");
        return ServiceResult<WorkoutDto>.Ok(_mapper.ToDto(workout, userId)!);
    }

    public async Task<ServiceResult<object>> CompleteWorkoutAsync(int workoutId, int clientUserId, int? perceivedIntensity, string? notes, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<object>.NotFound("Client profile not found.");

        var cw = await _db.ClientWorkouts.FirstOrDefaultAsync(cw => cw.WorkoutId == workoutId && cw.ClientId == client.Id, ct);
        if (cw is null) return ServiceResult<object>.NotFound("Workout assignment not found.");

        cw.CompletedUtc = DateTime.UtcNow;
        cw.PerceivedIntensity = perceivedIntensity;
        cw.ClientNotes = notes;
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }
}
