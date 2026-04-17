using Fitmaniac.Application.Common;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Measurements;

namespace Fitmaniac.Application.Services;

public interface IMeasurementService
{
    Task<ServiceResult<IReadOnlyList<MeasurementDto>>> GetMyMeasurementsAsync(int clientUserId, GoalUnit? unit, DateTime? from, DateTime? to, CancellationToken ct = default);
    Task<ServiceResult<MeasurementDto>> CreateAsync(CreateMeasurementDto dto, CancellationToken ct = default);
    Task<ServiceResult<MeasurementDto>> UpdateAsync(UpdateMeasurementDto dto, int requestingUserId, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteAsync(int id, int requestingUserId, CancellationToken ct = default);
}
