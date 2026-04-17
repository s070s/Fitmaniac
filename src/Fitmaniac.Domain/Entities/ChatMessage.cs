using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class ChatMessage : AuditableEntity
{
    public int ConversationId { get; set; }
    public ChatConversation Conversation { get; set; } = null!;

    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;

    [Required, MaxLength(2000)] public string Content { get; set; } = null!;
    [Required] public UserRole SenderRole { get; set; }
    [Required] public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReadAt { get; set; }
}
