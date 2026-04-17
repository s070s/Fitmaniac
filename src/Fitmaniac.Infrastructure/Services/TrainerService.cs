using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Trainers;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class TrainerService : ITrainerService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public TrainerService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PagedResult<TrainerListItemDto>>> GetTrainersAsync(int page, int pageSize, string? sortBy, bool descending, TrainerSpecialization? specialization, CancellationToken ct = default)
    {
        var query = _db.Trainers.Include(t => t.User).AsQueryable();

        if (specialization.HasValue)
            query = query.Where(t => t.Specializations.Contains(specialization.Value));

        query = (sortBy?.ToLower(), descending) switch
        {
            ("name", false) => query.OrderBy(t => t.FirstName),
            ("name", true) => query.OrderByDescending(t => t.FirstName),
            _ => query.OrderBy(t => t.Id),
        };

        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        var dtos = items.Select(t => _mapper.ToListItemDto(t)!).ToList();

        return ServiceResult<PagedResult<TrainerListItemDto>>.Ok(new PagedResult<TrainerListItemDto>(dtos, total, page, pageSize));
    }

    public async Task<ServiceResult<TrainerDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var trainer = await _db.Trainers
            .Include(t => t.User)
            .Include(t => t.Workouts)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

        if (trainer is null) return ServiceResult<TrainerDto>.NotFound("Trainer not found.");
        return ServiceResult<TrainerDto>.Ok(_mapper.ToDto(trainer)!);
    }
}
