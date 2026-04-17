using Fitmaniac.Application.Common;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Trainers;

namespace Fitmaniac.Web.Consumers;

public interface ITrainerConsumer
{
    Task<PagedResult<TrainerListItemDto>?> GetTrainersAsync(int page = 1, int pageSize = 20, TrainerSpecialization? specialization = null, CancellationToken ct = default);
    Task<TrainerDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TrainerDto?> CreateTrainerAsync(CreateTrainerDto dto, CancellationToken ct = default);
    Task<TrainerDto?> UpdateTrainerAsync(int id, UpdateTrainerProfileDto dto, CancellationToken ct = default);
    Task<bool> DeleteTrainerAsync(int id, CancellationToken ct = default);
}

public sealed class TrainerConsumer(HttpClient http) : ApiClientBase(http), ITrainerConsumer
{
    public Task<PagedResult<TrainerListItemDto>?> GetTrainersAsync(int page = 1, int pageSize = 20, TrainerSpecialization? specialization = null, CancellationToken ct = default) =>
        GetAsync<PagedResult<TrainerListItemDto>>($"/api/trainers?page={page}&pageSize={pageSize}&specialization={specialization}", ct);

    public Task<TrainerDto?> GetByIdAsync(int id, CancellationToken ct = default) =>
        GetAsync<TrainerDto>($"/api/trainers/{id}", ct);

    public Task<TrainerDto?> CreateTrainerAsync(CreateTrainerDto dto, CancellationToken ct = default) =>
        PostAsync<TrainerDto>("/api/admin/trainers", dto, ct);

    public Task<TrainerDto?> UpdateTrainerAsync(int id, UpdateTrainerProfileDto dto, CancellationToken ct = default) =>
        PutAsync<TrainerDto>($"/api/admin/trainers/{id}", dto, ct);

    public Task<bool> DeleteTrainerAsync(int id, CancellationToken ct = default) =>
        DeleteAsync($"/api/admin/trainers/{id}", ct);
}
