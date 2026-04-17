using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.Clients;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class ClientService : IClientService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public ClientService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<object>> SubscribeToTrainerAsync(int clientUserId, int trainerId, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<object>.NotFound("Client profile not found.");

        var trainer = await _db.Trainers.FindAsync([trainerId], ct);
        if (trainer is null) return ServiceResult<object>.NotFound("Trainer not found.");

        var exists = await _db.TrainerClients.AnyAsync(tc => tc.TrainerId == trainerId && tc.ClientId == client.Id, ct);
        if (exists) return ServiceResult<object>.Conflict("Already subscribed to this trainer.");

        _db.TrainerClients.Add(new TrainerClient
        {
            TrainerId = trainerId,
            ClientId = client.Id,
            SubscribedUtc = DateTime.UtcNow,
        });
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.Created(new { message = "Subscribed." });
    }

    public async Task<ServiceResult<object>> UnsubscribeFromTrainerAsync(int clientUserId, int trainerId, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<object>.NotFound("Client profile not found.");

        var tc = await _db.TrainerClients.FirstOrDefaultAsync(tc => tc.TrainerId == trainerId && tc.ClientId == client.Id, ct);
        if (tc is null) return ServiceResult<object>.NotFound("Subscription not found.");

        tc.UnsubscribedUtc = DateTime.UtcNow;
        _db.TrainerClients.Remove(tc);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }

    public async Task<ServiceResult<IReadOnlyList<int>>> GetSubscriptionsAsync(int clientUserId, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<IReadOnlyList<int>>.NotFound("Client profile not found.");

        var ids = await _db.TrainerClients
            .Where(tc => tc.ClientId == client.Id)
            .Select(tc => tc.TrainerId)
            .ToListAsync(ct);

        return ServiceResult<IReadOnlyList<int>>.Ok(ids);
    }

    public async Task<ServiceResult<ClientDto>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var client = await _db.Clients
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

        if (client is null) return ServiceResult<ClientDto>.NotFound("Client not found.");
        return ServiceResult<ClientDto>.Ok(_mapper.ToDto(client)!);
    }
}
