using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.DTOs.Measurements;
using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Endpoints;

public static class MeasurementEndpoints
{
    public static IEndpointRouteBuilder MapMeasurementEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/measurements").RequireAuthorization();

        g.MapGet("/me", async ([FromQuery] GoalUnit? unit, [FromQuery] DateTime? from, [FromQuery] DateTime? to,
            ICurrentUserService cur, IMeasurementService svc, CancellationToken ct) =>
            (await svc.GetMyMeasurementsAsync(cur.UserId!.Value, unit, from, to, ct)).ToResult());

        g.MapPost("/", async (CreateMeasurementDto dto, IMeasurementService svc, CancellationToken ct) =>
            (await svc.CreateAsync(dto, ct)).ToResult());

        g.MapPut("/", async (UpdateMeasurementDto dto, ICurrentUserService cur, IMeasurementService svc, CancellationToken ct) =>
            (await svc.UpdateAsync(dto, cur.UserId!.Value, ct)).ToResult());

        g.MapDelete("/{id:int}", async (int id, ICurrentUserService cur, IMeasurementService svc, CancellationToken ct) =>
            (await svc.DeleteAsync(id, cur.UserId!.Value, ct)).ToResult());

        return e;
    }
}
