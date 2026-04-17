namespace Fitmaniac.Shared.DTOs.Chat;

public sealed record ChatConversationDto(
    int Id,
    int CreatorId,
    string CreatorName,
    int ParticipantId,
    string ParticipantName,
    string? LastMessageContent,
    DateTime? LastMessageAt);

public sealed record ChatMessageDto(
    int Id,
    int ConversationId,
    int UserId,
    string SenderName,
    string Content,
    string SenderRole,
    DateTime SentAt,
    DateTime? ReadAt);

public sealed record SendMessageRequestDto(string Content);

public sealed record StartConversationRequestDto(int TargetUserId);
