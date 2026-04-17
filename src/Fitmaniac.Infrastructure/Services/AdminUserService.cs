using Fitmaniac.Application.Common;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Domain.Extensions;
using Fitmaniac.Shared.Constants;
using Fitmaniac.Shared.DTOs.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class AdminUserService : IAdminUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IModelToDtoMapper _mapper;

    public AdminUserService(UserManager<ApplicationUser> userManager, IModelToDtoMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    public async Task<ServiceResult<PagedResult<UserDto>>> GetUsersAsync(int page, int pageSize, string? sortBy, bool descending, string? search, CancellationToken ct = default)
    {
        var query = _userManager.Users.SearchByTerm(search);

        query = (sortBy?.ToLower(), descending) switch
        {
            ("email", false) => query.OrderBy(u => u.Email),
            ("email", true) => query.OrderByDescending(u => u.Email),
            ("username", false) => query.OrderBy(u => u.UserName),
            ("username", true) => query.OrderByDescending(u => u.UserName),
            (_, false) => query.OrderBy(u => u.CreatedUtc),
            _ => query.OrderByDescending(u => u.CreatedUtc),
        };

        var total = await query.CountAsync(ct);
        var skip = (page - 1) * pageSize;
        var items = await query.Skip(skip).Take(pageSize).ToListAsync(ct);
        var dtos = items.Select(u => _mapper.ToDto(u)!).ToList();

        return ServiceResult<PagedResult<UserDto>>.Ok(new PagedResult<UserDto>(dtos, total, page, pageSize));
    }

    public async Task<ServiceResult<UserStatisticsDto>> GetStatisticsAsync(CancellationToken ct = default)
    {
        var users = _userManager.Users;
        var stats = new UserStatisticsDto
        {
            TotalUsers = await users.CountAsync(ct),
            ActiveUsers = await users.CountAsync(u => u.Status == UserStatus.Active, ct),
            PendingUsers = await users.CountAsync(u => u.Status == UserStatus.Pending, ct),
            DisabledUsers = await users.CountAsync(u => u.Status == UserStatus.Disabled, ct),
            TotalTrainers = await users.CountAsync(u => u.Role == UserRole.Trainer, ct),
            TotalClients = await users.CountAsync(u => u.Role == UserRole.Client, ct),
        };
        return ServiceResult<UserStatisticsDto>.Ok(stats);
    }

    public async Task<ServiceResult<UserDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        if (user is null) return ServiceResult<UserDto>.NotFound("User not found.");
        return ServiceResult<UserDto>.Ok(_mapper.ToDto(user)!);
    }

    public async Task<ServiceResult<UserDto>> CreateAsync(CreateUserDto dto, CancellationToken ct = default)
    {
        var user = new ApplicationUser
        {
            UserName = dto.UserName,
            Email = dto.Email,
            Role = dto.Role,
            Status = UserStatus.Active,
            IsEnabled = true,
            EmailConfirmed = true,
        };
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return ServiceResult<UserDto>.BadRequest(string.Join("; ", result.Errors.Select(e => e.Description)));

        var roleName = dto.Role switch
        {
            UserRole.Admin => RoleNames.Admin,
            UserRole.Trainer => RoleNames.Trainer,
            _ => RoleNames.Client,
        };
        await _userManager.AddToRoleAsync(user, roleName);
        return ServiceResult<UserDto>.Created(_mapper.ToDto(user)!);
    }

    public async Task<ServiceResult<UserDto>> UpdateAsync(UpdateUserDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(dto.Id.ToString());
        if (user is null) return ServiceResult<UserDto>.NotFound("User not found.");
        user.UserName = dto.UserName ?? user.UserName;
        user.Email = dto.Email ?? user.Email;
        if (dto.Role.HasValue) user.Role = dto.Role.Value;
        if (dto.Status.HasValue) user.Status = dto.Status.Value;
        await _userManager.UpdateAsync(user);
        return ServiceResult<UserDto>.Ok(_mapper.ToDto(user)!);
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return ServiceResult<object>.NotFound("User not found.");
        await _userManager.DeleteAsync(user);
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<object>> EnableAsync(int id, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return ServiceResult<object>.NotFound("User not found.");
        user.IsEnabled = true;
        user.Status = UserStatus.Active;
        await _userManager.UpdateAsync(user);
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<object>> DisableAsync(int id, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null) return ServiceResult<object>.NotFound("User not found.");
        user.IsEnabled = false;
        user.Status = UserStatus.Disabled;
        await _userManager.UpdateAsync(user);
        return ServiceResult<object>.NoContent();
    }
}
