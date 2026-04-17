using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Chat;

namespace Fitmaniac.Application.Services;

public interface IChatService
{
    Task<ServiceResult<ChatMessageDto>> SendMessageAsync(int conversationId, int senderId, string content, CancellationToken ct = default);
    Task<ServiceResult<ChatConversationDto>> StartConversationAsync(int creatorId, int targetUserId, CancellationToken ct = default);
    Task<ServiceResult<object>> MarkAsReadAsync(int conversationId, int readerUserId, CancellationToken ct = default);
}
