namespace Fitmaniac.Shared.DTOs.Programs;

public sealed record WeeklyProgramDto(
    int Id,
    string Name,
    string? Description,
    int DurationInWeeks,
    int CurrentWeek,
    bool IsCompleted,
    int ClientId);

public sealed record CreateWeeklyProgramDto(
    int ClientId,
    string Name,
    string? Description,
    int DurationInWeeks);

public sealed record UpdateWeeklyProgramDto(
    int Id,
    string? Name,
    string? Description,
    int? DurationInWeeks,
    int? CurrentWeek);
