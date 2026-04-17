using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Programs;

namespace Fitmaniac.Application.Services;

public interface IWeeklyProgramService
{
    Task<ServiceResult<WeeklyProgramDto?>> GetMyProgramAsync(int clientUserId, CancellationToken ct = default);
    Task<ServiceResult<WeeklyProgramDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<WeeklyProgramDto>> CreateAsync(CreateWeeklyProgramDto dto, CancellationToken ct = default);
    Task<ServiceResult<WeeklyProgramDto>> UpdateAsync(UpdateWeeklyProgramDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> AdvanceWeekAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default);
}
