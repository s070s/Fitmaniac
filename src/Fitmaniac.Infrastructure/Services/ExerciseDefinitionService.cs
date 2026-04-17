using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Exercises;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class ExerciseDefinitionService : IExerciseDefinitionService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public ExerciseDefinitionService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PagedResult<ExerciseDefinitionDto>>> GetExercisesAsync(int page, int pageSize, string? category, MuscleGroup? muscleGroup, ClientExperience? experience, int? equipmentId, CancellationToken ct = default)
    {
        var query = _db.ExerciseDefinitions.Include(e => e.Equipments).AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(e => e.Category == category);
        if (muscleGroup.HasValue)
            query = query.Where(e => e.PrimaryMuscleGroups.Contains(muscleGroup.Value) || e.SecondaryMuscleGroups.Contains(muscleGroup.Value));
        if (experience.HasValue)
            query = query.Where(e => e.ExperienceLevel == experience.Value);
        if (equipmentId.HasValue)
            query = query.Where(e => e.Equipments.Any(eq => eq.Id == equipmentId.Value));

        var total = await query.CountAsync(ct);
        var items = await query.OrderBy(e => e.Name).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return ServiceResult<PagedResult<ExerciseDefinitionDto>>.Ok(
            new PagedResult<ExerciseDefinitionDto>(items.Select(e => _mapper.ToDto(e)!).ToList(), total, page, pageSize));
    }

    public async Task<ServiceResult<ExerciseDefinitionDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var ex = await _db.ExerciseDefinitions.Include(e => e.Equipments).FirstOrDefaultAsync(e => e.Id == id, ct);
        if (ex is null) return ServiceResult<ExerciseDefinitionDto>.NotFound("Exercise not found.");
        return ServiceResult<ExerciseDefinitionDto>.Ok(_mapper.ToDto(ex)!);
    }

    public async Task<ServiceResult<ExerciseDefinitionDto>> CreateAsync(CreateExerciseDefinitionDto dto, CancellationToken ct = default)
    {
        var ex = new ExerciseDefinition
        {
            Name = dto.Name,
            Description = dto.Description,
            VideoUrl = dto.VideoUrl,
            CaloriesBurnedPerHour = dto.CaloriesBurnedPerHour,
            IsCompoundExercise = dto.IsCompoundExercise,
            PrimaryMuscleGroups = dto.PrimaryMuscleGroups?.ToList() ?? [],
            SecondaryMuscleGroups = dto.SecondaryMuscleGroups?.ToList() ?? [],
            ExperienceLevel = dto.ExperienceLevel,
            Category = dto.Category,
        };
        _db.ExerciseDefinitions.Add(ex);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<ExerciseDefinitionDto>.Created(_mapper.ToDto(ex)!);
    }

    public async Task<ServiceResult<ExerciseDefinitionDto>> UpdateAsync(UpdateExerciseDefinitionDto dto, CancellationToken ct = default)
    {
        var ex = await _db.ExerciseDefinitions.FindAsync([dto.Id], ct);
        if (ex is null) return ServiceResult<ExerciseDefinitionDto>.NotFound("Exercise not found.");

        ex.Name = dto.Name ?? ex.Name;
        ex.Description = dto.Description ?? ex.Description;
        ex.VideoUrl = dto.VideoUrl ?? ex.VideoUrl;
        ex.Category = dto.Category ?? ex.Category;

        await _db.SaveChangesAsync(ct);
        return ServiceResult<ExerciseDefinitionDto>.Ok(_mapper.ToDto(ex)!);
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var ex = await _db.ExerciseDefinitions.FindAsync([id], ct);
        if (ex is null) return ServiceResult<object>.NotFound("Exercise not found.");
        _db.ExerciseDefinitions.Remove(ex);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }
}
