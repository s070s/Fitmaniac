using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.Constants;
using Fitmaniac.Shared.DTOs.Exercises;
using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Endpoints;

public static class ExerciseEndpoints
{
    public static IEndpointRouteBuilder MapExerciseDefinitionEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/exercises").RequireAuthorization();

        g.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] string? category,
            [FromQuery] MuscleGroup? muscleGroup, [FromQuery] ClientExperience? experience, [FromQuery] int? equipmentId,
            IExerciseDefinitionService svc, CancellationToken ct) =>
            (await svc.GetExercisesAsync(page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, category, muscleGroup, experience, equipmentId, ct)).ToResult());

        g.MapGet("/{id:int}", async (int id, IExerciseDefinitionService svc, CancellationToken ct) =>
            (await svc.GetByIdAsync(id, ct)).ToResult());

        g.MapPost("/", async (CreateExerciseDefinitionDto dto, IExerciseDefinitionService svc, CancellationToken ct) =>
            (await svc.CreateAsync(dto, ct)).ToResult()).RequireAuthorization(PolicyNames.Admin);

        g.MapPut("/", async (UpdateExerciseDefinitionDto dto, IExerciseDefinitionService svc, CancellationToken ct) =>
            (await svc.UpdateAsync(dto, ct)).ToResult()).RequireAuthorization(PolicyNames.Admin);

        g.MapDelete("/{id:int}", async (int id, IExerciseDefinitionService svc, CancellationToken ct) =>
            (await svc.DeleteAsync(id, ct)).ToResult()).RequireAuthorization(PolicyNames.Admin);

        return e;
    }

    public static IEndpointRouteBuilder MapEquipmentEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/equipment").RequireAuthorization();

        g.MapGet("/", async (IEquipmentService svc, CancellationToken ct) =>
            (await svc.GetAllAsync(ct)).ToResult());

        g.MapPost("/", async (CreateEquipmentDto dto, IEquipmentService svc, CancellationToken ct) =>
            (await svc.CreateAsync(dto, ct)).ToResult()).RequireAuthorization(PolicyNames.Admin);

        g.MapPut("/{id:int}", async (int id, UpdateEquipmentDto dto, IEquipmentService svc, CancellationToken ct) =>
            (await svc.UpdateAsync(dto with { Id = id }, ct)).ToResult()).RequireAuthorization(PolicyNames.Admin);

        g.MapDelete("/{id:int}", async (int id, IEquipmentService svc, CancellationToken ct) =>
            (await svc.DeleteAsync(id, ct)).ToResult()).RequireAuthorization(PolicyNames.Admin);

        return e;
    }
}
