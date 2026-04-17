using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.Exercises;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class EquipmentService : IEquipmentService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public EquipmentService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<EquipmentDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _db.Equipments.OrderBy(e => e.Name).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<EquipmentDto>>.Ok(items.Select(e => _mapper.ToDto(e)!).ToList());
    }

    public async Task<ServiceResult<EquipmentDto>> CreateAsync(CreateEquipmentDto dto, CancellationToken ct = default)
    {
        var eq = new Equipment { Name = dto.Name, Description = dto.Description };
        _db.Equipments.Add(eq);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<EquipmentDto>.Created(_mapper.ToDto(eq)!);
    }

    public async Task<ServiceResult<EquipmentDto>> UpdateAsync(UpdateEquipmentDto dto, CancellationToken ct = default)
    {
        var eq = await _db.Equipments.FindAsync([dto.Id], ct);
        if (eq is null) return ServiceResult<EquipmentDto>.NotFound("Equipment not found.");
        eq.Name = dto.Name ?? eq.Name;
        eq.Description = dto.Description ?? eq.Description;
        await _db.SaveChangesAsync(ct);
        return ServiceResult<EquipmentDto>.Ok(_mapper.ToDto(eq)!);
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var eq = await _db.Equipments.FindAsync([id], ct);
        if (eq is null) return ServiceResult<object>.NotFound("Equipment not found.");
        _db.Equipments.Remove(eq);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }
}
