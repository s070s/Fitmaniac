using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Goals;
using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Endpoints;

public static class GoalEndpoints
{
    public static IEndpointRouteBuilder MapGoalEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/goals").RequireAuthorization();

        g.MapGet("/me", async (ICurrentUserService cur, IGoalService svc, CancellationToken ct) =>
            (await svc.GetMyGoalsAsync(cur.UserId!.Value, ct)).ToResult());

        g.MapPost("/", async (CreateGoalDto dto, IGoalService svc, CancellationToken ct) =>
            (await svc.CreateAsync(dto, ct)).ToResult());

        g.MapPut("/", async (UpdateGoalDto dto, ICurrentUserService cur, IGoalService svc, CancellationToken ct) =>
            (await svc.UpdateAsync(dto, cur.UserId!.Value, ct)).ToResult());

        g.MapDelete("/{id:int}", async (int id, ICurrentUserService cur, IGoalService svc, CancellationToken ct) =>
            (await svc.DeleteAsync(id, cur.UserId!.Value, ct)).ToResult());

        return e;
    }
}
