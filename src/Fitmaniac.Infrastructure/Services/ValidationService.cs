using Fitmaniac.Application.Abstractions;

namespace Fitmaniac.Infrastructure.Services;

public sealed class ValidationService : IValidationService
{
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];
    private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5 MB

    public bool IsValidImage(Stream stream, string fileName, long maxBytes = 2_097_152)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return false;
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(ext)) return false;
        if (stream.Length > maxBytes) return false;
        return true;
    }
}
