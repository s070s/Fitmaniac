using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Services;
using Fitmaniac.Shared.DTOs.Progress;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class ProgressService : IProgressService
{
    private readonly IApplicationDbContext _db;

    public ProgressService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ServiceResult<ProgressSummaryDto>> GetSummaryAsync(int clientUserId, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<ProgressSummaryDto>.NotFound("Client profile not found.");

        var totalWorkouts = await _db.ClientWorkouts.CountAsync(cw => cw.ClientId == client.Id, ct);
        var completedWorkouts = await _db.ClientWorkouts.CountAsync(cw => cw.ClientId == client.Id && cw.CompletedUtc != null, ct);
        var totalGoals = await _db.Goals.CountAsync(g => g.ClientId == client.Id, ct);
        var completedGoals = await _db.Goals.CountAsync(g => g.ClientId == client.Id && g.Status == Domain.Enums.GoalStatus.Completed, ct);

        var summary = new ProgressSummaryDto
        {
            TotalWorkouts = totalWorkouts,
            CompletedWorkouts = completedWorkouts,
            TotalGoals = totalGoals,
            CompletedGoals = completedGoals,
        };

        return ServiceResult<ProgressSummaryDto>.Ok(summary);
    }

    public async Task<ServiceResult<IReadOnlyList<WeeklyProgressDto>>> GetWeeklyAsync(int clientUserId, int weeks = 12, CancellationToken ct = default)
    {
        var client = await _db.Clients.FirstOrDefaultAsync(c => c.UserId == clientUserId, ct);
        if (client is null) return ServiceResult<IReadOnlyList<WeeklyProgressDto>>.NotFound("Client profile not found.");

        var from = DateTime.UtcNow.AddDays(-7 * weeks);
        var completions = await _db.ClientWorkouts
            .Where(cw => cw.ClientId == client.Id && cw.CompletedUtc != null && cw.CompletedUtc >= from)
            .Select(cw => cw.CompletedUtc!.Value)
            .ToListAsync(ct);

        var grouped = completions
            .GroupBy(d => new { Year = d.Year, Week = System.Globalization.ISOWeek.GetWeekOfYear(d) })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Week)
            .Select(g => new WeeklyProgressDto
            {
                WeekNumber = g.Key.Week,
                Year = g.Key.Year,
                CompletedWorkouts = g.Count(),
            })
            .ToList();

        return ServiceResult<IReadOnlyList<WeeklyProgressDto>>.Ok(grouped);
    }
}
