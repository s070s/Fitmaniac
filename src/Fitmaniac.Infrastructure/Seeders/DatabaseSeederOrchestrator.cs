using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fitmaniac.Infrastructure.Seeders;

public sealed class DatabaseSeederOrchestrator
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DatabaseSeederOrchestrator> _logger;

    public DatabaseSeederOrchestrator(IServiceProvider services, ILogger<DatabaseSeederOrchestrator> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async Task SeedAllAsync()
    {
        _logger.LogInformation("Starting database seeding...");

        using var scope = _services.CreateScope();
        var sp = scope.ServiceProvider;

        await sp.GetRequiredService<IdentityRoleSeeder>().SeedAsync();
        await sp.GetRequiredService<AdminUserSeeder>().SeedAsync();
        await sp.GetRequiredService<SubscriptionPlanSeeder>().SeedAsync();
        await sp.GetRequiredService<EquipmentSeeder>().SeedAsync();
        await sp.GetRequiredService<ExerciseDefinitionSeeder>().SeedAsync();

        _logger.LogInformation("Database seeding complete.");
    }
}
