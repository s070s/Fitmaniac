namespace Fitmaniac.MAUI.Services;

public interface IProgressService
{
    Task<ProgressSummaryDto?> GetSummaryAsync(CancellationToken ct = default);
    Task<IReadOnlyList<WeeklyProgressDto>?> GetWeeklyAsync(CancellationToken ct = default);
}

public sealed class ProgressService(IApiClient api) : IProgressService
{
    public Task<ProgressSummaryDto?> GetSummaryAsync(CancellationToken ct = default) =>
        api.GetAsync<ProgressSummaryDto>("/api/progress/summary", ct);

    public Task<IReadOnlyList<WeeklyProgressDto>?> GetWeeklyAsync(CancellationToken ct = default) =>
        api.GetAsync<IReadOnlyList<WeeklyProgressDto>>("/api/progress/weekly", ct);
}
