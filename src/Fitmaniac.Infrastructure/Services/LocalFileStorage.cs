using Fitmaniac.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;

namespace Fitmaniac.Infrastructure.Services;

public sealed class LocalFileStorage : IFileStorage
{
    private readonly string _root;
    private readonly IWebHostEnvironment _env;

    public LocalFileStorage(IWebHostEnvironment env)
    {
        _env = env;
        _root = Path.Combine(env.WebRootPath ?? Path.GetTempPath(), "uploads");
        Directory.CreateDirectory(_root);
    }

    public async Task<string> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken ct = default)
    {
        var safe = Path.GetFileName(fileName);
        var unique = $"{Guid.NewGuid():N}_{safe}";
        var fullPath = Path.Combine(_root, unique);
        await using var fs = File.Create(fullPath);
        await stream.CopyToAsync(fs, ct);
        return $"/uploads/{unique}";
    }

    public Task DeleteAsync(string path, CancellationToken ct = default)
    {
        var filename = Path.GetFileName(path);
        var fullPath = Path.Combine(_root, filename);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }

    public bool Exists(string path)
    {
        var filename = Path.GetFileName(path);
        return File.Exists(Path.Combine(_root, filename));
    }
}
