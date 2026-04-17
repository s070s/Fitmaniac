using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.Workouts;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class AdminWorkoutService : IAdminWorkoutService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public AdminWorkoutService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PagedResult<WorkoutListItemDto>>> GetWorkoutsAsync(int page, int pageSize, int? trainerId, CancellationToken ct = default)
    {
        var query = _db.Workouts.AsQueryable();
        if (trainerId.HasValue) query = query.Where(w => w.TrainerId == trainerId.Value);

        var total = await query.CountAsync(ct);
        var items = await query.OrderByDescending(w => w.ScheduledDateTime)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return ServiceResult<PagedResult<WorkoutListItemDto>>.Ok(
            new PagedResult<WorkoutListItemDto>(items.Select(w => _mapper.ToListItemDto(w)!).ToList(), total, page, pageSize));
    }

    public async Task<ServiceResult<WorkoutDto>> CreateAsync(CreateWorkoutDto dto, CancellationToken ct = default)
    {
        var workout = new Workout
        {
            ScheduledDateTime = dto.ScheduledDateTime,
            Type = dto.Type,
            DurationInMinutes = dto.DurationInMinutes,
            Notes = dto.Notes,
            TrainerId = dto.TrainerId,
            WeeklyProgramId = dto.WeeklyProgramId,
        };
        _db.Workouts.Add(workout);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<WorkoutDto>.Created(_mapper.ToDto(workout)!);
    }

    public async Task<ServiceResult<WorkoutDto>> UpdateAsync(int id, UpdateWorkoutDto dto, CancellationToken ct = default)
    {
        var workout = await _db.Workouts.FindAsync([id], ct);
        if (workout is null) return ServiceResult<WorkoutDto>.NotFound("Workout not found.");

        workout.ScheduledDateTime = dto.ScheduledDateTime ?? workout.ScheduledDateTime;
        workout.Type = dto.Type ?? workout.Type;
        workout.DurationInMinutes = dto.DurationInMinutes ?? workout.DurationInMinutes;
        workout.Notes = dto.Notes ?? workout.Notes;

        await _db.SaveChangesAsync(ct);
        return ServiceResult<WorkoutDto>.Ok(_mapper.ToDto(workout)!);
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var workout = await _db.Workouts.FindAsync([id], ct);
        if (workout is null) return ServiceResult<object>.NotFound("Workout not found.");
        _db.Workouts.Remove(workout);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<WorkoutExerciseDto>> AddExerciseAsync(CreateWorkoutExerciseDto dto, CancellationToken ct = default)
    {
        var we = new WorkoutExercise
        {
            WorkoutId = dto.WorkoutId,
            ExerciseDefinitionId = dto.ExerciseDefinitionId,
            Notes = dto.Notes,
        };
        _db.WorkoutExercises.Add(we);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<WorkoutExerciseDto>.Created(_mapper.ToDto(we)!);
    }

    public async Task<ServiceResult<WorkoutExerciseDto>> UpdateExerciseAsync(UpdateWorkoutExerciseDto dto, CancellationToken ct = default)
    {
        var we = await _db.WorkoutExercises.FindAsync([dto.Id], ct);
        if (we is null) return ServiceResult<WorkoutExerciseDto>.NotFound("WorkoutExercise not found.");
        we.Notes = dto.Notes ?? we.Notes;
        await _db.SaveChangesAsync(ct);
        return ServiceResult<WorkoutExerciseDto>.Ok(_mapper.ToDto(we)!);
    }

    public async Task<ServiceResult<object>> RemoveExerciseAsync(int workoutExerciseId, CancellationToken ct = default)
    {
        var we = await _db.WorkoutExercises.FindAsync([workoutExerciseId], ct);
        if (we is null) return ServiceResult<object>.NotFound("WorkoutExercise not found.");
        _db.WorkoutExercises.Remove(we);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<WorkoutExerciseSetDto>> AddSetAsync(CreateWorkoutExerciseSetDto dto, CancellationToken ct = default)
    {
        var set = new WorkoutExerciseSet
        {
            WorkoutExerciseId = dto.WorkoutExerciseId,
            SetNumber = dto.SetNumber,
            Repetitions = dto.Repetitions,
            Weight = dto.Weight,
        };
        _db.WorkoutExerciseSets.Add(set);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<WorkoutExerciseSetDto>.Created(_mapper.ToDto(set)!);
    }

    public async Task<ServiceResult<WorkoutExerciseSetDto>> UpdateSetAsync(UpdateWorkoutExerciseSetDto dto, CancellationToken ct = default)
    {
        var set = await _db.WorkoutExerciseSets.FindAsync([dto.Id], ct);
        if (set is null) return ServiceResult<WorkoutExerciseSetDto>.NotFound("Set not found.");
        set.Repetitions = dto.Repetitions ?? set.Repetitions;
        set.Weight = dto.Weight ?? set.Weight;
        await _db.SaveChangesAsync(ct);
        return ServiceResult<WorkoutExerciseSetDto>.Ok(_mapper.ToDto(set)!);
    }

    public async Task<ServiceResult<object>> DeleteSetAsync(int setId, CancellationToken ct = default)
    {
        var set = await _db.WorkoutExerciseSets.FindAsync([setId], ct);
        if (set is null) return ServiceResult<object>.NotFound("Set not found.");
        _db.WorkoutExerciseSets.Remove(set);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }
}
