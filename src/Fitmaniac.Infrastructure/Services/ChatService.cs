using Fitmaniac.Application.Common;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Shared.DTOs.Chat;
using Microsoft.EntityFrameworkCore;

namespace Fitmaniac.Infrastructure.Services;

public sealed class ChatService : IChatService
{
    private readonly IApplicationDbContext _db;
    private readonly IModelToDtoMapper _mapper;

    public ChatService(IApplicationDbContext db, IModelToDtoMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<ChatMessageDto>> SendMessageAsync(int conversationId, int senderId, string content, CancellationToken ct = default)
    {
        var convo = await _db.ChatConversations.FindAsync([conversationId], ct);
        if (convo is null) return ServiceResult<ChatMessageDto>.NotFound("Conversation not found.");

        if (convo.CreatorId != senderId && convo.ParticipantId != senderId)
            return ServiceResult<ChatMessageDto>.Forbidden("Not a participant.");

        var msg = new ChatMessage
        {
            ConversationId = conversationId,
            UserId = senderId,
            Content = new Ganss.Xss.HtmlSanitizer().Sanitize(content),
            SentAt = DateTime.UtcNow,
        };
        _db.ChatMessages.Add(msg);

        convo.LastMessageContent = msg.Content;
        convo.LastMessageAt = msg.SentAt;

        await _db.SaveChangesAsync(ct);
        return ServiceResult<ChatMessageDto>.Created(_mapper.ToDto(msg)!);
    }

    public async Task<ServiceResult<ChatConversationDto>> StartConversationAsync(int creatorId, int targetUserId, CancellationToken ct = default)
    {
        var existing = await _db.ChatConversations
            .FirstOrDefaultAsync(c =>
                (c.CreatorId == creatorId && c.ParticipantId == targetUserId) ||
                (c.CreatorId == targetUserId && c.ParticipantId == creatorId), ct);

        if (existing is not null)
            return ServiceResult<ChatConversationDto>.Ok(_mapper.ToDto(existing)!);

        var convo = new ChatConversation
        {
            CreatorId = creatorId,
            ParticipantId = targetUserId,
            LastMessageAt = DateTime.UtcNow,
        };
        _db.ChatConversations.Add(convo);
        await _db.SaveChangesAsync(ct);
        return ServiceResult<ChatConversationDto>.Created(_mapper.ToDto(convo)!);
    }

    public async Task<ServiceResult<object>> MarkAsReadAsync(int conversationId, int readerUserId, CancellationToken ct = default)
    {
        var unread = await _db.ChatMessages
            .Where(m => m.ConversationId == conversationId && m.UserId != readerUserId && m.ReadAt == null)
            .ToListAsync(ct);

        foreach (var m in unread) m.ReadAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return ServiceResult<object>.NoContent();
    }
}
