using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;

namespace Fitmaniac.Api.Endpoints;

public static class ClientEndpoints
{
    public static IEndpointRouteBuilder MapClientEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/clients").RequireAuthorization();

        g.MapGet("/me/subscriptions", async (ICurrentUserService cur, IClientService svc, CancellationToken ct) =>
            (await svc.GetSubscriptionsAsync(cur.UserId!.Value, ct)).ToResult());

        g.MapGet("/{id:int}", async (int id, IClientService svc, CancellationToken ct) =>
            (await svc.GetByIdAsync(id, ct)).ToResult());

        g.MapPost("/me/subscribe/{trainerId:int}", async (int trainerId, ICurrentUserService cur, IClientService svc, CancellationToken ct) =>
            (await svc.SubscribeToTrainerAsync(cur.UserId!.Value, trainerId, ct)).ToResult());

        g.MapDelete("/me/subscribe/{trainerId:int}", async (int trainerId, ICurrentUserService cur, IClientService svc, CancellationToken ct) =>
            (await svc.UnsubscribeFromTrainerAsync(cur.UserId!.Value, trainerId, ct)).ToResult());

        return e;
    }
}
