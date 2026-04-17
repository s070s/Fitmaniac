using Fitmaniac.Application.Common;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Trainers;

namespace Fitmaniac.Application.Services;

public interface ITrainerService
{
    Task<ServiceResult<PagedResult<TrainerListItemDto>>> GetTrainersAsync(int page, int pageSize, string? sortBy, bool descending, TrainerSpecialization? specialization, CancellationToken ct = default);
    Task<ServiceResult<TrainerDto>> GetByIdAsync(int id, CancellationToken ct = default);
}
