using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Clients;

namespace Fitmaniac.Application.Services;

public interface IClientService
{
    Task<ServiceResult<object>> SubscribeToTrainerAsync(int clientUserId, int trainerId, CancellationToken ct = default);
    Task<ServiceResult<object>> UnsubscribeFromTrainerAsync(int clientUserId, int trainerId, CancellationToken ct = default);
    Task<ServiceResult<IReadOnlyList<int>>> GetSubscriptionsAsync(int clientUserId, CancellationToken ct = default);
    Task<ServiceResult<ClientDto>> GetByIdAsync(int id, CancellationToken ct = default);
}
