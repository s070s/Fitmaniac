using Fitmaniac.Application.Common;
using Fitmaniac.Shared.DTOs.Chat;

namespace Fitmaniac.Application.Services;

public interface IChatQueryService
{
    Task<ServiceResult<IReadOnlyList<ChatConversationDto>>> GetConversationsAsync(int userId, CancellationToken ct = default);
    Task<ServiceResult<PagedResult<ChatMessageDto>>> GetMessagesAsync(int conversationId, int userId, int page, int pageSize, CancellationToken ct = default);
}
