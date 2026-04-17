using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;
using Fitmaniac.Shared.Constants;
using Fitmaniac.Shared.DTOs.Programs;

namespace Fitmaniac.Api.Endpoints;

public static class ProgramEndpoints
{
    public static IEndpointRouteBuilder MapWeeklyProgramEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/programs").RequireAuthorization();

        g.MapGet("/me", async (ICurrentUserService cur, IWeeklyProgramService svc, CancellationToken ct) =>
            (await svc.GetMyProgramAsync(cur.UserId!.Value, ct)).ToResult());

        g.MapGet("/{id:int}", async (int id, IWeeklyProgramService svc, CancellationToken ct) =>
            (await svc.GetByIdAsync(id, ct)).ToResult());

        g.MapPost("/", async (CreateWeeklyProgramDto dto, IWeeklyProgramService svc, CancellationToken ct) =>
            (await svc.CreateAsync(dto, ct)).ToResult()).RequireAuthorization(PolicyNames.StaffOrOwner);

        g.MapPut("/", async (UpdateWeeklyProgramDto dto, IWeeklyProgramService svc, CancellationToken ct) =>
            (await svc.UpdateAsync(dto, ct)).ToResult()).RequireAuthorization(PolicyNames.StaffOrOwner);

        g.MapPost("/{id:int}/advance", async (int id, IWeeklyProgramService svc, CancellationToken ct) =>
            (await svc.AdvanceWeekAsync(id, ct)).ToResult()).RequireAuthorization(PolicyNames.StaffOrOwner);

        g.MapDelete("/{id:int}", async (int id, IWeeklyProgramService svc, CancellationToken ct) =>
            (await svc.DeleteAsync(id, ct)).ToResult()).RequireAuthorization(PolicyNames.Admin);

        return e;
    }
}
