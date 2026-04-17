using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.MedicalHistory;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class MedicalHistoryService : IMedicalHistoryService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public MedicalHistoryService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<MedicalHistoryDto?>> GetMyHistoryAsync(int clientUserId, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<MedicalHistoryDto?>.NotFound("Client profile not found.");

        var history = await _db.MedicalHistories.FirstOrDefaultAsync(h => h.ClientId == client.Id, ct);
        return ServiceResult<MedicalHistoryDto?>.Ok(_mapper.ToDto(history));
    }

    public async Task<ServiceResult<MedicalHistoryDto>> UpsertAsync(int clientUserId, UpdateMedicalHistoryDto dto, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<MedicalHistoryDto>.NotFound("Client profile not found.");

        var history = await _db.MedicalHistories.FirstOrDefaultAsync(h => h.ClientId == client.Id, ct);
        if (history is null)
        {
            history = new MedicalHistory { ClientId = client.Id };
            _db.MedicalHistories.Add(history);
        }

        history.Description = dto.Description ?? history.Description;
        if (dto.Conditions is not null) history.Conditions = dto.Conditions.ToList();
        if (dto.MedicationTypes is not null) history.MedicationTypes = dto.MedicationTypes.ToList();
        if (dto.Surgeries is not null) history.Surgeries = dto.Surgeries.ToList();
        if (dto.RecommendedIntensityLevel.HasValue) history.RecommendedIntensityLevel = dto.RecommendedIntensityLevel;

        await _db.SaveChangesAsync(ct);
        return ServiceResult<MedicalHistoryDto>.Ok(_mapper.ToDto(history)!);
    }
}
