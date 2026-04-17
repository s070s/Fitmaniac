using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Measurements;

namespace Fitmaniac.Web.Consumers;

public interface IMeasurementConsumer
{
    Task<IReadOnlyList<MeasurementDto>?> GetMyMeasurementsAsync(GoalUnit? unit = null, DateTime? from = null, DateTime? to = null, CancellationToken ct = default);
    Task<MeasurementDto?> CreateAsync(CreateMeasurementDto dto, CancellationToken ct = default);
    Task<MeasurementDto?> UpdateAsync(UpdateMeasurementDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}

public sealed class MeasurementConsumer(HttpClient http) : ApiClientBase(http), IMeasurementConsumer
{
    public Task<IReadOnlyList<MeasurementDto>?> GetMyMeasurementsAsync(GoalUnit? unit = null, DateTime? from = null, DateTime? to = null, CancellationToken ct = default) =>
        GetAsync<IReadOnlyList<MeasurementDto>>($"/api/measurements/me?unit={unit}&from={from:O}&to={to:O}", ct);

    public Task<MeasurementDto?> CreateAsync(CreateMeasurementDto dto, CancellationToken ct = default) =>
        PostAsync<MeasurementDto>("/api/measurements", dto, ct);

    public Task<MeasurementDto?> UpdateAsync(UpdateMeasurementDto dto, CancellationToken ct = default) =>
        PutAsync<MeasurementDto>("/api/measurements", dto, ct);

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default) =>
        DeleteAsync($"/api/measurements/{id}", ct);
}
