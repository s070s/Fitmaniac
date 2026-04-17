using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class ChatConversation : AuditableEntity
{
    public int CreatorId { get; set; }
    public ApplicationUser Creator { get; set; } = null!;

    public int ParticipantId { get; set; }
    public ApplicationUser Participant { get; set; } = null!;

    [MaxLength(500)] public string? LastMessageContent { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
