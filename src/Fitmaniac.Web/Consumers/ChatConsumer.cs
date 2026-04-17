using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Chat;

namespace Fitmaniac.Web.Consumers;

public interface IChatConsumer
{
    Task<IReadOnlyList<ChatConversationDto>?> GetConversationsAsync(CancellationToken ct = default);
    Task<PagedResult<ChatMessageDto>?> GetMessagesAsync(int conversationId, int page = 1, int pageSize = 20, CancellationToken ct = default);
    Task<ChatConversationDto?> StartConversationAsync(int targetUserId, CancellationToken ct = default);
    Task<ChatMessageDto?> SendMessageAsync(int conversationId, string content, CancellationToken ct = default);
    Task<bool> MarkAsReadAsync(int conversationId, CancellationToken ct = default);
}

public sealed class ChatConsumer(HttpClient http) : ApiClientBase(http), IChatConsumer
{
    public Task<IReadOnlyList<ChatConversationDto>?> GetConversationsAsync(CancellationToken ct = default) =>
        GetAsync<IReadOnlyList<ChatConversationDto>>("/api/chat/conversations", ct);

    public Task<PagedResult<ChatMessageDto>?> GetMessagesAsync(int conversationId, int page = 1, int pageSize = 20, CancellationToken ct = default) =>
        GetAsync<PagedResult<ChatMessageDto>>($"/api/chat/conversations/{conversationId}/messages?page={page}&pageSize={pageSize}", ct);

    public Task<ChatConversationDto?> StartConversationAsync(int targetUserId, CancellationToken ct = default) =>
        PostAsync<ChatConversationDto>($"/api/chat/conversations/{targetUserId}", null, ct);

    public Task<ChatMessageDto?> SendMessageAsync(int conversationId, string content, CancellationToken ct = default) =>
        PostAsync<ChatMessageDto>($"/api/chat/conversations/{conversationId}/messages", new { Content = content }, ct);

    public Task<bool> MarkAsReadAsync(int conversationId, CancellationToken ct = default) =>
        PostVoidAsync($"/api/chat/conversations/{conversationId}/read", null, ct);
}
