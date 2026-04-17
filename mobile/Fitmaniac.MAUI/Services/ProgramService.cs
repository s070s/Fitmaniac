using Fitmaniac.Shared.DTOs.Programs;

namespace Fitmaniac.MAUI.Services;

public interface IProgramService
{
    Task<WeeklyProgramDto?> GetMyProgramAsync(CancellationToken ct = default);
}

public sealed class ProgramService(IApiClient api) : IProgramService
{
    public Task<WeeklyProgramDto?> GetMyProgramAsync(CancellationToken ct = default) =>
        api.GetAsync<WeeklyProgramDto>("/api/programs/me", ct);
}
