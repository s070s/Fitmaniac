using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Users;

namespace Fitmaniac.Web.Consumers;

public interface IUserConsumer
{
    Task<UserDto?> GetCurrentUserAsync(CancellationToken ct = default);
    Task<UserDto?> UpdateProfileAsync(UpdateUserDto dto, CancellationToken ct = default);
    Task<PagedResult<UserDto>?> GetUsersAsync(int page = 1, int pageSize = 20, string? sortBy = null, bool descending = false, string? search = null, CancellationToken ct = default);
    Task<UserStatisticsDto?> GetStatisticsAsync(CancellationToken ct = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<UserDto?> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default);
    Task<UserDto?> UpdateUserAsync(UpdateUserDto dto, CancellationToken ct = default);
    Task<bool> DeleteUserAsync(int id, CancellationToken ct = default);
    Task<bool> EnableUserAsync(int id, CancellationToken ct = default);
    Task<bool> DisableUserAsync(int id, CancellationToken ct = default);
}

public sealed class UserConsumer(HttpClient http) : ApiClientBase(http), IUserConsumer
{
    public Task<UserDto?> GetCurrentUserAsync(CancellationToken ct = default) =>
        GetAsync<UserDto>("/api/users/me", ct);

    public Task<UserDto?> UpdateProfileAsync(UpdateUserDto dto, CancellationToken ct = default) =>
        PutAsync<UserDto>("/api/users/me", dto, ct);

    public Task<PagedResult<UserDto>?> GetUsersAsync(int page = 1, int pageSize = 20, string? sortBy = null, bool descending = false, string? search = null, CancellationToken ct = default) =>
        GetAsync<PagedResult<UserDto>>($"/api/admin/users?page={page}&pageSize={pageSize}&sortBy={sortBy}&descending={descending}&search={search}", ct);

    public Task<UserStatisticsDto?> GetStatisticsAsync(CancellationToken ct = default) =>
        GetAsync<UserStatisticsDto>("/api/admin/users/statistics", ct);

    public Task<UserDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        GetAsync<UserDto>($"/api/admin/users/{id}", ct);

    public Task<UserDto?> CreateUserAsync(CreateUserDto dto, CancellationToken ct = default) =>
        PostAsync<UserDto>("/api/admin/users", dto, ct);

    public Task<UserDto?> UpdateUserAsync(UpdateUserDto dto, CancellationToken ct = default) =>
        PutAsync<UserDto>("/api/admin/users", dto, ct);

    public Task<bool> DeleteUserAsync(int id, CancellationToken ct = default) =>
        DeleteAsync($"/api/admin/users/{id}", ct);

    public Task<bool> EnableUserAsync(int id, CancellationToken ct = default) =>
        PostVoidAsync($"/api/admin/users/{id}/enable", null, ct);

    public Task<bool> DisableUserAsync(int id, CancellationToken ct = default) =>
        PostVoidAsync($"/api/admin/users/{id}/disable", null, ct);
}
