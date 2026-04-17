using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Fitmaniac.Infrastructure.Seeders;

public sealed class IdentityRoleSeeder
{
    private readonly RoleManager<IdentityRole<int>> _roleManager;
    private readonly ILogger<IdentityRoleSeeder> _logger;

    public IdentityRoleSeeder(RoleManager<IdentityRole<int>> roleManager, ILogger<IdentityRoleSeeder> logger)
    {
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        string[] roles = [RoleNames.Admin, RoleNames.Trainer, RoleNames.Client];
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<int>(role));
                _logger.LogInformation("Created role: {Role}", role);
            }
        }
    }
}
