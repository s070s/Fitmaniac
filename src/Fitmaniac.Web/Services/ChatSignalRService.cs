using Fitmaniac.Shared.DTOs.Chat;
using Microsoft.AspNetCore.SignalR.Client;

namespace Fitmaniac.Web.Services;

public sealed class ChatSignalRService : IAsyncDisposable
{
    private HubConnection? _hub;
    public event Action<ChatMessageDto>? MessageReceived;
    public event Action<ChatConversationDto>? ConversationStarted;

    public async Task ConnectAsync(string accessToken, string hubUrl)
    {
        _hub = new HubConnectionBuilder()
            .WithUrl(hubUrl, opts => opts.AccessTokenProvider = () => Task.FromResult(accessToken)!)
            .WithAutomaticReconnect()
            .Build();

        _hub.On<ChatMessageDto>("ReceiveMessage", msg => MessageReceived?.Invoke(msg));
        _hub.On<ChatConversationDto>("ConversationStarted", conv => ConversationStarted?.Invoke(conv));

        await _hub.StartAsync();
    }

    public async Task SendMessageAsync(int conversationId, string content)
    {
        if (_hub?.State == HubConnectionState.Connected)
            await _hub.InvokeAsync("SendMessage", conversationId, content);
    }

    public async Task InitiateConversationAsync(int targetUserId)
    {
        if (_hub?.State == HubConnectionState.Connected)
            await _hub.InvokeAsync("InitiateConversation", targetUserId);
    }

    public async ValueTask DisposeAsync()
    {
        if (_hub is not null) await _hub.DisposeAsync();
    }
}
