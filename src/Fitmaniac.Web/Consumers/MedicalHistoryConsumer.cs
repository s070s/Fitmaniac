using Fitmaniac.Shared.DTOs.MedicalHistory;

namespace Fitmaniac.Web.Consumers;

public interface IMedicalHistoryConsumer
{
    Task<MedicalHistoryDto?> GetMyHistoryAsync(CancellationToken ct = default);
    Task<MedicalHistoryDto?> UpsertAsync(UpdateMedicalHistoryDto dto, CancellationToken ct = default);
}

public sealed class MedicalHistoryConsumer(HttpClient http) : ApiClientBase(http), IMedicalHistoryConsumer
{
    public Task<MedicalHistoryDto?> GetMyHistoryAsync(CancellationToken ct = default) =>
        GetAsync<MedicalHistoryDto>("/api/medical-history/me", ct);

    public Task<MedicalHistoryDto?> UpsertAsync(UpdateMedicalHistoryDto dto, CancellationToken ct = default) =>
        PutAsync<MedicalHistoryDto>("/api/medical-history/me", dto, ct);
}
