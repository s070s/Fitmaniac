using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;
using Fitmaniac.Shared.DTOs.MedicalHistory;

namespace Fitmaniac.Api.Endpoints;

public static class MedicalHistoryEndpoints
{
    public static IEndpointRouteBuilder MapMedicalHistoryEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/medical-history").RequireAuthorization();

        g.MapGet("/me", async (ICurrentUserService cur, IMedicalHistoryService svc, CancellationToken ct) =>
            (await svc.GetMyHistoryAsync(cur.UserId!.Value, ct)).ToResult());

        g.MapPut("/me", async (UpdateMedicalHistoryDto dto, ICurrentUserService cur, IMedicalHistoryService svc, CancellationToken ct) =>
            (await svc.UpsertAsync(cur.UserId!.Value, dto, ct)).ToResult());

        return e;
    }
}
