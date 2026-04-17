using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Progress;

namespace Fitmaniac.Application.Services;

public interface IProgressService
{
    Task<ServiceResult<ProgressSummaryDto>> GetSummaryAsync(int clientUserId, CancellationToken ct = default);
    Task<ServiceResult<IReadOnlyList<WeeklyProgressDto>>> GetWeeklyAsync(int clientUserId, int weeks = 12, CancellationToken ct = default);
}
