using Fitmaniac.Shared.DTOs.Measurements;

namespace Fitmaniac.MAUI.Services;

public interface IMeasurementService
{
    Task<IReadOnlyList<MeasurementDto>?> GetMyMeasurementsAsync(CancellationToken ct = default);
    Task<MeasurementDto?> CreateAsync(CreateMeasurementDto dto, CancellationToken ct = default);
}

public sealed class MeasurementService(IApiClient api) : IMeasurementService
{
    public Task<IReadOnlyList<MeasurementDto>?> GetMyMeasurementsAsync(CancellationToken ct = default) =>
        api.GetAsync<IReadOnlyList<MeasurementDto>>("/api/measurements/me", ct);

    public Task<MeasurementDto?> CreateAsync(CreateMeasurementDto dto, CancellationToken ct = default) =>
        api.PostAsync<MeasurementDto>("/api/measurements", dto, ct);
}
