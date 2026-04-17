using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fitmaniac.Api.Endpoints;

public static class ChatEndpoints
{
    public static IEndpointRouteBuilder MapChatEndpoints(this IEndpointRouteBuilder e)
    {
        var g = e.MapGroup("/chat").RequireAuthorization();

        g.MapGet("/conversations", async (ICurrentUserService cur, IChatQueryService svc, CancellationToken ct) =>
            (await svc.GetConversationsAsync(cur.UserId!.Value, ct)).ToResult());

        g.MapGet("/conversations/{conversationId:int}/messages", async (
            int conversationId, [FromQuery] int page, [FromQuery] int pageSize,
            ICurrentUserService cur, IChatQueryService svc, CancellationToken ct) =>
            (await svc.GetMessagesAsync(conversationId, cur.UserId!.Value, page == 0 ? 1 : page, pageSize == 0 ? 20 : pageSize, ct)).ToResult());

        g.MapPost("/conversations/{targetUserId:int}", async (int targetUserId, ICurrentUserService cur, IChatService svc, CancellationToken ct) =>
            (await svc.StartConversationAsync(cur.UserId!.Value, targetUserId, ct)).ToResult());

        g.MapPost("/conversations/{conversationId:int}/messages", async (
            int conversationId, [FromBody] SendMessageRequest req, ICurrentUserService cur, IChatService svc, CancellationToken ct) =>
            (await svc.SendMessageAsync(conversationId, cur.UserId!.Value, req.Content, ct)).ToResult());

        g.MapPost("/conversations/{conversationId:int}/read", async (int conversationId, ICurrentUserService cur, IChatService svc, CancellationToken ct) =>
            (await svc.MarkAsReadAsync(conversationId, cur.UserId!.Value, ct)).ToResult());

        return e;
    }
}

public sealed record SendMessageRequest(string Content);
