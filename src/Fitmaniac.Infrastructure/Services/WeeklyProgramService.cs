using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.Programs;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class WeeklyProgramService : IWeeklyProgramService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public WeeklyProgramService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<WeeklyProgramDto?>> GetMyProgramAsync(int clientUserId, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<WeeklyProgramDto?>.NotFound("Client profile not found.");

        var program = await _db.WeeklyPrograms
            .Include(p => p.Workouts)
            .FirstOrDefaultAsync(p => p.ClientId == client.Id, ct);

        return ServiceResult<WeeklyProgramDto?>.Ok(_mapper.ToDto(program));
    }

    public async Task<ServiceResult<WeeklyProgramDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var p = await _db.WeeklyPrograms.Include(p => p.Workouts).FirstOrDefaultAsync(p => p.Id == id, ct);
        if (p is null) return ServiceResult<WeeklyProgramDto>.NotFound("Program not found.");
        return ServiceResult<WeeklyProgramDto>.Ok(_mapper.ToDto(p)!);
    }

    public async Task<ServiceResult<WeeklyProgramDto>> CreateAsync(CreateWeeklyProgramDto dto, CancellationToken ct = default)
    {
        var client = await _db.Clients.FindAsync([dto.ClientId], ct);
        if (client is null) return ServiceResult<WeeklyProgramDto>.NotFound("Client profile not found.");

        var program = new WeeklyProgram
        {
            Name = dto.Name,
            Description = dto.Description,
            DurationInWeeks = dto.DurationInWeeks,
            CurrentWeek = 1,
            ClientId = dto.ClientId,
        };
        _db.WeeklyPrograms.Add(program);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<WeeklyProgramDto>.Created(_mapper.ToDto(program)!);
    }

    public async Task<ServiceResult<WeeklyProgramDto>> UpdateAsync(UpdateWeeklyProgramDto dto, CancellationToken ct = default)
    {
        var p = await _db.WeeklyPrograms.FindAsync([dto.Id], ct);
        if (p is null) return ServiceResult<WeeklyProgramDto>.NotFound("Program not found.");
        p.Name = dto.Name ?? p.Name;
        p.Description = dto.Description ?? p.Description;
        await _db.SaveChangesAsync(ct);
        return ServiceResult<WeeklyProgramDto>.Ok(_mapper.ToDto(p)!);
    }

    public async Task<ServiceResult<object>> AdvanceWeekAsync(int id, CancellationToken ct = default)
    {
        var p = await _db.WeeklyPrograms.FindAsync([id], ct);
        if (p is null) return ServiceResult<object>.NotFound("Program not found.");
        if (p.IsCompleted) return ServiceResult<object>.BadRequest("Program is already completed.");
        p.CurrentWeek++;
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.Ok(new { currentWeek = p.CurrentWeek, isCompleted = p.IsCompleted });
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var p = await _db.WeeklyPrograms.FindAsync([id], ct);
        if (p is null) return ServiceResult<object>.NotFound("Program not found.");
        _db.WeeklyPrograms.Remove(p);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }
}
