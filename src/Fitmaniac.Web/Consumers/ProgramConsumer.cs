using Fitmaniac.Shared.DTOs.Programs;

namespace Fitmaniac.Web.Consumers;

public interface IProgramConsumer
{
    Task<WeeklyProgramDto?> GetMyProgramAsync(CancellationToken ct = default);
    Task<WeeklyProgramDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<WeeklyProgramDto?> CreateAsync(CreateWeeklyProgramDto dto, CancellationToken ct = default);
    Task<WeeklyProgramDto?> UpdateAsync(UpdateWeeklyProgramDto dto, CancellationToken ct = default);
    Task<bool> AdvanceWeekAsync(int id, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public sealed class ProgramConsumer(HttpClient http) : ApiClientBase(http), IProgramConsumer
{
    public Task<WeeklyProgramDto?> GetMyProgramAsync(CancellationToken ct = default) =>
        GetAsync<WeeklyProgramDto>("/api/programs/me", ct);

    public Task<WeeklyProgramDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        GetAsync<WeeklyProgramDto>($"/api/programs/{id}", ct);

    public Task<WeeklyProgramDto?> CreateAsync(CreateWeeklyProgramDto dto, CancellationToken ct = default) =>
        PostAsync<WeeklyProgramDto>("/api/programs", dto, ct);

    public Task<WeeklyProgramDto?> UpdateAsync(UpdateWeeklyProgramDto dto, CancellationToken ct = default) =>
        PutAsync<WeeklyProgramDto>("/api/programs", dto, ct);

    public Task<bool> AdvanceWeekAsync(int id, CancellationToken ct = default) =>
        PostVoidAsync($"/api/programs/{id}/advance", null, ct);

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default) =>
        DeleteAsync($"/api/programs/{id}", ct);
}
