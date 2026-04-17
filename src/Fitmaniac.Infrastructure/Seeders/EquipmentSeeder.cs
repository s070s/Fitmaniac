using Fitmaniac.Application.Data;
using Fitmaniac.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace Fitmaniac.Infrastructure.Seeders;

public sealed class EquipmentSeeder
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<EquipmentSeeder> _logger;

    public EquipmentSeeder(IApplicationDbContext db, ILogger<EquipmentSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _db.Equipments.AnyAsync()) return;

        var json = ReadEmbedded("Fitmaniac.Shared.JSON.Equipment.equipment.json");
        if (json is null) return;

        var items = JsonSerializer.Deserialize<List<EquipmentSeedItem>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (items is null) return;

        foreach (var item in items)
        {
            _db.Equipments.Add(new Equipment { Name = item.Name, Description = item.Description });
        }
        await _db.SaveChangesAsync();
        _logger.LogInformation("Equipment seeded: {Count} items.", items.Count);
    }

    private static string? ReadEmbedded(string resourceName)
    {
        var assembly = Assembly.Load("Fitmaniac.Shared");
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null) return null;
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private sealed record EquipmentSeedItem(string Name, string? Description);
}
