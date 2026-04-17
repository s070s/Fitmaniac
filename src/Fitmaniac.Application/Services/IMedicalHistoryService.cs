using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.MedicalHistory;

namespace Fitmaniac.Application.Services;

public interface IMedicalHistoryService
{
    Task<ServiceResult<MedicalHistoryDto?>> GetMyHistoryAsync(int clientUserId, CancellationToken ct = default);
    Task<ServiceResult<MedicalHistoryDto>> UpsertAsync(int clientUserId, UpdateMedicalHistoryDto dto, CancellationToken ct = default);
}
