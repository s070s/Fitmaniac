using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Trainers;

namespace Fitmaniac.Application.Services;

public interface IAdminTrainerService
{
    Task<ServiceResult<TrainerDto>> CreateAsync(CreateTrainerDto dto, CancellationToken ct = default);
    Task<ServiceResult<TrainerDto>> UpdateAsync(int id, UpdateTrainerProfileDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default);
}
