using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.Constants;
using Fitmaniac.Shared.DTOs.Trainers;
using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Endpoints;

public static class TrainerEndpoints
{
    public static IEndpointRouteBuilder MapTrainerEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/trainers").RequireAuthorization();

        g.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? sortBy,
            [FromQuery] bool descending, [FromQuery] TrainerSpecialization? specialization,
            ITrainerService svc, CancellationToken ct) =>
            (await svc.GetTrainersAsync(page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, sortBy, descending, specialization, ct)).ToResult());

        g.MapGet("/{id:int}", async (int id, ITrainerService svc, CancellationToken ct) =>
            (await svc.GetByIdAsync(id, ct)).ToResult());

        return e;
    }

    public static IEndpointRouteBuilder MapAdminTrainerEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/admin/trainers").RequireAuthorization(PolicyNames.Admin);

        g.MapPost("/", async (CreateTrainerDto dto, IAdminTrainerService svc, CancellationToken ct) =>
            (await svc.CreateAsync(dto, ct)).ToResult());

        g.MapPut("/{id:int}", async (int id, UpdateTrainerProfileDto dto, IAdminTrainerService svc, CancellationToken ct) =>
            (await svc.UpdateAsync(id, dto, ct)).ToResult());

        g.MapDelete("/{id:int}", async (int id, IAdminTrainerService svc, CancellationToken ct) =>
            (await svc.DeleteAsync(id, ct)).ToResult());

        return e;
    }
}
