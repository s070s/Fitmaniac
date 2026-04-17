using Fitmaniac.Shared.DTOs.Progress;

namespace Fitmaniac.Web.Consumers;

public interface IProgressConsumer
{
    Task<ProgressSummaryDto?> GetSummaryAsync(CancellationToken ct = default);
    Task<IReadOnlyList<WeeklyProgressDto>?> GetWeeklyAsync(int weeks = 12, CancellationToken ct = default);
}

public sealed class ProgressConsumer(HttpClient http) : ApiClientBase(http), IProgressConsumer
{
    public Task<ProgressSummaryDto?> GetSummaryAsync(CancellationToken ct = default) =>
        GetAsync<ProgressSummaryDto>("/api/progress/summary", ct);

    public Task<IReadOnlyList<WeeklyProgressDto>?> GetWeeklyAsync(int weeks = 12, CancellationToken ct = default) =>
        GetAsync<IReadOnlyList<WeeklyProgressDto>>($"/api/progress/weekly?weeks={weeks}", ct);
}
