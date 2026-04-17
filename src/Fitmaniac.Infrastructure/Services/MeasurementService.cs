using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Measurements;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class MeasurementService : IMeasurementService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public MeasurementService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<MeasurementDto>>> GetMyMeasurementsAsync(int clientUserId, GoalUnit? unit, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<IReadOnlyList<MeasurementDto>>.NotFound("Client profile not found.");

        var query = _db.Measurements.Where(m => m.ClientId == client.Id);
        if (unit.HasValue) query = query.Where(m => m.Unit == unit.Value);
        if (from.HasValue) query = query.Where(m => m.Date >= from.Value);
        if (to.HasValue) query = query.Where(m => m.Date <= to.Value);

        var items = await query.OrderByDescending(m => m.Date).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<MeasurementDto>>.Ok(items.Select(m => _mapper.ToDto(m)!).ToList());
    }

    public async Task<ServiceResult<MeasurementDto>> CreateAsync(CreateMeasurementDto dto, CancellationToken ct = default)
    {
        var client = await _db.Clients.FindAsync([dto.ClientId], ct);
        if (client is null) return ServiceResult<MeasurementDto>.NotFound("Client profile not found.");

        var measurement = new Measurement
        {
            Unit = dto.Unit,
            Value = dto.Value,
            Date = dto.Date,
            Notes = dto.Notes,
            ClientId = dto.ClientId,
        };
        _db.Measurements.Add(measurement);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<MeasurementDto>.Created(_mapper.ToDto(measurement)!);
    }

    public async Task<ServiceResult<MeasurementDto>> UpdateAsync(UpdateMeasurementDto dto, int requestingUserId, CancellationToken ct = default)
    {
        var m = await _db.Measurements.Include(m => m.Client).FirstOrDefaultAsync(m => m.Id == dto.Id, ct);
        if (m is null) return ServiceResult<MeasurementDto>.NotFound("Measurement not found.");
        if (m.Client.UserId != requestingUserId) return ServiceResult<MeasurementDto>.Forbidden("Not authorized.");

        m.Value = dto.Value ?? m.Value;
        m.Notes = dto.Notes ?? m.Notes;
        await _db.SaveChangesAsync(ct);
        return ServiceResult<MeasurementDto>.Ok(_mapper.ToDto(m)!);
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, int requestingUserId, CancellationToken ct = default)
    {
        var m = await _db.Measurements.Include(m => m.Client).FirstOrDefaultAsync(m => m.Id == id, ct);
        if (m is null) return ServiceResult<object>.NotFound("Measurement not found.");
        if (m.Client.UserId != requestingUserId) return ServiceResult<object>.Forbidden("Not authorized.");
        _db.Measurements.Remove(m);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }
}
