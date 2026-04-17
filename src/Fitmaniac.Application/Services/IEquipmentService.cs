using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Exercises;

namespace Fitmaniac.Application.Services;

public interface IEquipmentService
{
    Task<ServiceResult<IReadOnlyList<EquipmentDto>>> GetAllAsync(CancellationToken ct = default);
    Task<ServiceResult<EquipmentDto>> CreateAsync(CreateEquipmentDto dto, CancellationToken ct = default);
    Task<ServiceResult<EquipmentDto>> UpdateAsync(UpdateEquipmentDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default);
}
