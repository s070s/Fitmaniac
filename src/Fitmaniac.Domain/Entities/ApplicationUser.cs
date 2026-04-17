using Microsoft.AspNetCore.Identity;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public UserRole Role { get; set; } = UserRole.Client;
    public UserStatus Status { get; set; } = UserStatus.Unknown;
    public bool IsEnabled { get; set; } = true;
    public bool GdprConsentGiven { get; set; }
    public DateTime? GdprConsentDate { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginUtc { get; set; }

    public Trainer? TrainerProfile { get; set; }
    public Client? ClientProfile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
