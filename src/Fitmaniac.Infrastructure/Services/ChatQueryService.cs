using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Shared.DTOs.Chat;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class ChatQueryService : IChatQueryService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public ChatQueryService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IReadOnlyList<ChatConversationDto>>> GetConversationsAsync(int userId, CancellationToken ct = default)
    {
        var convos = await _db.ChatConversations
            .Where(c => c.CreatorId == userId || c.ParticipantId == userId)
            .OrderByDescending(c => c.LastMessageAt)
            .ToListAsync(ct);

        return ServiceResult<IReadOnlyList<ChatConversationDto>>.Ok(convos.Select(c => _mapper.ToDto(c)!).ToList());
    }

    public async Task<ServiceResult<PagedResult<ChatMessageDto>>> GetMessagesAsync(int conversationId, int userId, int page, int pageSize, CancellationToken ct = default)
    {
        var convo = await _db.ChatConversations.FindAsync([conversationId], ct);
        if (convo is null) return ServiceResult<PagedResult<ChatMessageDto>>.NotFound("Conversation not found.");
        if (convo.CreatorId != userId && convo.ParticipantId != userId)
            return ServiceResult<PagedResult<ChatMessageDto>>.Forbidden("Not a participant.");

        var query = _db.ChatMessages.Where(m => m.ConversationId == conversationId).OrderByDescending(m => m.SentAt);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        return ServiceResult<PagedResult<ChatMessageDto>>.Ok(
            new PagedResult<ChatMessageDto>(items.Select(m => _mapper.ToDto(m)!).ToList(), total, page, pageSize));
    }
}
