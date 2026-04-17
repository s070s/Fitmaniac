using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Users;

namespace Fitmaniac.Application.Services;

public interface IAdminUserService
{
    Task<ServiceResult<PagedResult<UserDto>>> GetUsersAsync(int page, int pageSize, string? sortBy, bool descending, string? search, CancellationToken ct = default);
    Task<ServiceResult<UserStatisticsDto>> GetStatisticsAsync(CancellationToken ct = default);
    Task<ServiceResult<UserDto>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<UserDto>> CreateAsync(CreateUserDto dto, CancellationToken ct = default);
    Task<ServiceResult<UserDto>> UpdateAsync(UpdateUserDto dto, CancellationToken ct = default);
    Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<object>> EnableAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<object>> DisableAsync(int id, CancellationToken ct = default);
}
