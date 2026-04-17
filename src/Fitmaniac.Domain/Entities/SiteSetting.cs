using System.ComponentModel.DataAnnotations;

namespace Fitmaniac.Domain.Entities;

public class SiteSetting
{
    [Key, MaxLength(100)] public string Key { get; set; } = null!;
    public string? Value { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedUtc { get; set; } = DateTime.UtcNow;
}
