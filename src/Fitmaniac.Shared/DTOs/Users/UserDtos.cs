using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Trainers;
using Fitmaniac.Shared.DTOs.Clients;

namespace Fitmaniac.Shared.DTOs.Users;

public sealed record UserDto(
    int Id,
    string Username,
    string Email,
    string Role,
    bool IsEnabled,
    UserStatus Status,
    DateTime CreatedUtc,
    DateTime? LastLoginUtc,
    TrainerDto? TrainerProfile,
    ClientDto? ClientProfile);

public sealed record CreateUserDto(
    string UserName,
    string Email,
    string Password,
    UserRole Role,
    bool IsEnabled = true);

public sealed record UpdateUserDto(
    int Id,
    string? UserName,
    string? Email,
    bool? IsEnabled,
    UserRole? Role,
    UserStatus? Status);

public sealed record UserStatisticsDto
{
    public int TotalUsers { get; init; }
    public int ActiveUsers { get; init; }
    public int PendingUsers { get; init; }
    public int DisabledUsers { get; init; }
    public int TotalTrainers { get; init; }
    public int TotalClients { get; init; }
};
