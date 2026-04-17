namespace Fitmaniac.Application.Abstractions;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream stream, string fileName, string folder, CancellationToken ct = default);
    Task DeleteAsync(string filePath, CancellationToken ct = default);
    bool Exists(string filePath);
}
