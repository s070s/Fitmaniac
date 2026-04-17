using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Endpoints;

public static class SubscriptionEndpoints
{
    public static IEndpointRouteBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/subscriptions").RequireAuthorization();

        g.MapGet("/plans", async (ISubscriptionService svc, CancellationToken ct) =>
            (await svc.GetPlansAsync(ct)).ToResult());

        g.MapGet("/me", async (ICurrentUserService cur, ISubscriptionService svc, CancellationToken ct) =>
            (await svc.GetCurrentSubscriptionAsync(cur.UserId!.Value, ct)).ToResult());

        g.MapPost("/upgrade/{planId:int}", async (int planId, ICurrentUserService cur, ISubscriptionService svc, CancellationToken ct) =>
            (await svc.UpgradeAsync(cur.UserId!.Value, planId, ct)).ToResult());

        return e;
    }

    public static IEndpointRouteBuilder MapBillingEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/billing").RequireAuthorization();

        g.MapGet("/transactions", async ([FromQuery] int page, [FromQuery] int pageSize,
            ICurrentUserService cur, IBillingService svc, CancellationToken ct) =>
            (await svc.GetTransactionsAsync(cur.UserId!.Value, page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, ct)).ToResult());

        return e;
    }
}
