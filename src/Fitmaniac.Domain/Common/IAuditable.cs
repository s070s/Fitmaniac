namespace Fitmaniac.Domain.Common;

public interface IAuditable
{
    DateTime CreatedUtc { get; set; }
    DateTime UpdatedUtc { get; set; }
    string? CreatedBy { get; set; }
    string? UpdatedBy { get; set; }
}
