using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Endpoints;

public static class ProgressEndpoints
{
    public static IEndpointRouteBuilder MapProgressEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/progress").RequireAuthorization();

        g.MapGet("/summary", async (ICurrentUserService cur, IProgressService svc, CancellationToken ct) =>
            (await svc.GetSummaryAsync(cur.UserId!.Value, ct)).ToResult());

        g.MapGet("/weekly", async ([FromQuery] int weeks, ICurrentUserService cur, IProgressService svc, CancellationToken ct) =>
            (await svc.GetWeeklyAsync(cur.UserId!.Value, weeks == 0 ? 12 : weeks, ct)).ToResult());

        return e;
    }
}
