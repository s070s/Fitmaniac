using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Goals;

namespace Fitmaniac.Application.Services;

public interface IGoalService
{
    Task<ServiceResult<IReadOnlyList<GoalDto>>> GetMyGoalsAsync(int clientUserId, CancellationToken ct = default);
    Task<ServiceResult<GoalDto>> CreateAsync(CreateGoalDto dto, CancellationToken ct = default);
    Task<ServiceResult<GoalDto>> UpdateAsync(UpdateGoalDto dto, int requestingUserId, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteAsync(int id, int requestingUserId, CancellationToken ct = default);
}
