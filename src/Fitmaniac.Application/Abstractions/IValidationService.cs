namespace Fitmaniac.Application.Abstractions;

public interface IValidationService
{
    bool IsValidImage(Stream stream, string fileName, long maxBytes = 2_097_152);
}
