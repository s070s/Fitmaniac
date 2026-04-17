using Fitmaniac.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Fitmaniac.Api.Hubs;

[Authorize]
public sealed class ChatHub(IChatService chatService) : Hub
{
    public async Task SendMessage(int conversationId, string content)
    {
        var userId = GetUserId();
        var result = await chatService.SendMessageAsync(conversationId, userId, content);
        if (result.IsSuccess && result.Value is not null)
        {
            await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", result.Value);
        }
    }

    public async Task InitiateConversation(int targetUserId)
    {
        var userId = GetUserId();
        var result = await chatService.StartConversationAsync(userId, targetUserId);
        if (result.IsSuccess && result.Value is not null)
        {
            var group = result.Value.Id.ToString();
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            await Clients.Caller.SendAsync("ConversationStarted", result.Value);
        }
    }

    public async Task MarkRead(int conversationId)
    {
        var userId = GetUserId();
        await chatService.MarkAsReadAsync(conversationId, userId);
        await Clients.Group(conversationId.ToString()).SendAsync("MessageRead", conversationId, userId);
    }

    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
    }

    private int GetUserId()
    {
        var claim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)
                  ?? Context.User?.FindFirst("sub");
        return int.Parse(claim?.Value ?? "0");
    }
}
