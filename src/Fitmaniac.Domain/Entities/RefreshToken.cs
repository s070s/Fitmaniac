using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public class RefreshToken : AuditableEntity
{
    [Required, MaxLength(128)] public string TokenHash { get; set; } = null!;
    [MaxLength(128)] public string? ReplacedByTokenHash { get; set; }
    [Required] public DateTime ExpiresUtc { get; set; }
    public DateTime? RevokedUtc { get; set; }
    [MaxLength(45)] public string? CreatedByIp { get; set; }
    [MaxLength(45)] public string? RevokedByIp { get; set; }

    [NotMapped] public bool IsExpired => DateTime.UtcNow >= ExpiresUtc;
    [NotMapped] public bool IsActive => RevokedUtc is null && !IsExpired;

    public int UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
}
