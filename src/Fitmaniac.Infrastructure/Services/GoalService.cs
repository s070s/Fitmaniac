using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.Goals;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class GoalService : IGoalService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public GoalService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<GoalDto>>> GetMyGoalsAsync(int clientUserId, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<IReadOnlyList<GoalDto>>.NotFound("Client profile not found.");

        var goals = await _db.Goals.Where(g => g.ClientId == client.Id).OrderByDescending(g => g.CreatedUtc).ToListAsync(ct);
        return ServiceResult<IReadOnlyList<GoalDto>>.Ok(goals.Select(g => _mapper.ToDto(g)!).ToList());
    }

    public async Task<ServiceResult<GoalDto>> CreateAsync(CreateGoalDto dto, CancellationToken ct = default)
    {
        var client = await _db.Clients.FindAsync([dto.ClientId], ct);
        if (client is null) return ServiceResult<GoalDto>.NotFound("Client profile not found.");

        var goal = new Goal
        {
            GoalType = dto.GoalType,
            Description = dto.Description,
            TargetDate = dto.TargetDate,
            GoalQuantity = dto.GoalQuantity,
            GoalUnit = dto.GoalUnit,
            ClientId = dto.ClientId,
        };
        _db.Goals.Add(goal);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<GoalDto>.Created(_mapper.ToDto(goal)!);
    }

    public async Task<ServiceResult<GoalDto>> UpdateAsync(UpdateGoalDto dto, int requestingUserId, CancellationToken ct = default)
    {
        var goal = await _db.Goals.Include(g => g.Client).FirstOrDefaultAsync(g => g.Id == dto.Id, ct);
        if (goal is null) return ServiceResult<GoalDto>.NotFound("Goal not found.");
        if (goal.Client.UserId != requestingUserId) return ServiceResult<GoalDto>.Forbidden("Not authorized.");

        goal.Description = dto.Description ?? goal.Description;
        goal.TargetDate = dto.TargetDate ?? goal.TargetDate;
        if (dto.Status.HasValue) goal.Status = dto.Status.Value;
        if (dto.GoalQuantity.HasValue) goal.GoalQuantity = dto.GoalQuantity.Value;

        await _db.SaveChangesAsync(ct);
        return ServiceResult<GoalDto>.Ok(_mapper.ToDto(goal)!);
    }

    public async Task<ServiceResult<object>> DeleteAsync(int id, int requestingUserId, CancellationToken ct = default)
    {
        var goal = await _db.Goals.Include(g => g.Client).FirstOrDefaultAsync(g => g.Id == id, ct);
        if (goal is null) return ServiceResult<object>.NotFound("Goal not found.");
        if (goal.Client.UserId != requestingUserId) return ServiceResult<object>.Forbidden("Not authorized.");

        _db.Goals.Remove(goal);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }
}
