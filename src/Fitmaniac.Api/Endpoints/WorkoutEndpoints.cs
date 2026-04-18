using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;
using Fitmaniac.Shared.Constants;
using Fitmaniac.Shared.DTOs.Workouts;
using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Endpoints;

public static class WorkoutEndpoints
{
    public static IEndpointRouteBuilder MapWorkoutEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/workouts").RequireAuthorization();

        g.MapGet("/", async ([FromQuery] string? from, [FromQuery] string? to, [FromQuery] int? programId,
            ICurrentUserService cur, IWorkoutService svc, CancellationToken ct) =>
        {
            if (!QueryParameterParsers.TryParseOptionalDateTime(from, "from", out var parsedFrom, out var fromErrors))
            {
                return Results.ValidationProblem(fromErrors!);
            }

            if (!QueryParameterParsers.TryParseOptionalDateTime(to, "to", out var parsedTo, out var toErrors))
            {
                return Results.ValidationProblem(toErrors!);
            }

            return (await svc.GetMyWorkoutsAsync(cur.UserId!.Value, parsedFrom, parsedTo, programId, ct)).ToResult();
        });

        g.MapGet("/{id:int}", async (int id, ICurrentUserService cur, IWorkoutService svc, CancellationToken ct) =>
            (await svc.GetByIdAsync(id, cur.UserId!.Value, ct)).ToResult());

        g.MapPost("/{id:int}/complete", async (int id, [FromQuery] int? perceivedIntensity,
            [FromQuery] string? notes, ICurrentUserService cur, IWorkoutService svc, CancellationToken ct) =>
            (await svc.CompleteWorkoutAsync(id, cur.UserId!.Value, perceivedIntensity, notes, ct)).ToResult());

        return e;
    }

    public static IEndpointRouteBuilder MapAdminWorkoutEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/admin/workouts").RequireAuthorization(PolicyNames.StaffOrOwner);

        g.MapGet("/", async ([FromQuery] int page, [FromQuery] int pageSize, [FromQuery] int? trainerId,
            IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.GetWorkoutsAsync(page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, trainerId, ct)).ToResult());

        g.MapPost("/", async (CreateWorkoutDto dto, IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.CreateAsync(dto, ct)).ToResult());

        g.MapPut("/{id:int}", async (int id, UpdateWorkoutDto dto, IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.UpdateAsync(id, dto, ct)).ToResult());

        g.MapDelete("/{id:int}", async (int id, IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.DeleteAsync(id, ct)).ToResult());

        g.MapPost("/exercises", async (CreateWorkoutExerciseDto dto, IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.AddExerciseAsync(dto, ct)).ToResult());

        g.MapPut("/exercises", async (UpdateWorkoutExerciseDto dto, IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.UpdateExerciseAsync(dto, ct)).ToResult());

        g.MapDelete("/exercises/{workoutExerciseId:int}", async (int workoutExerciseId, IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.RemoveExerciseAsync(workoutExerciseId, ct)).ToResult());

        g.MapPost("/sets", async (CreateWorkoutExerciseSetDto dto, IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.AddSetAsync(dto, ct)).ToResult());

        g.MapPut("/sets", async (UpdateWorkoutExerciseSetDto dto, IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.UpdateSetAsync(dto, ct)).ToResult());

        g.MapDelete("/sets/{setId:int}", async (int setId, IAdminWorkoutService svc, CancellationToken ct) =>
            (await svc.DeleteSetAsync(setId, ct)).ToResult());

        return e;
    }
}
