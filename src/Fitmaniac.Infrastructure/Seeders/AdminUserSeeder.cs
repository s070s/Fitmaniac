using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Fitmaniac.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fitmaniac.Infrastructure.Seeders;

public sealed class AdminUserSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AdminUserSeeder> _logger;

    public AdminUserSeeder(UserManager<ApplicationUser> userManager, IConfiguration config, ILogger<AdminUserSeeder> logger)
    {
        _userManager = userManager;
        _config = config;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        var email = _config["Seed:AdminEmail"] ?? SeedDefaults.AdminEmailFallback;
        var password = _config["Seed:AdminPassword"] ?? SeedDefaults.AdminPasswordFallback;
        var username = _config["Seed:AdminUsername"] ?? SeedDefaults.AdminUsernameFallback;

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null) return;

        var admin = new ApplicationUser
        {
            UserName = username,
            Email = email,
            EmailConfirmed = true,
            Role = UserRole.Admin,
            Status = UserStatus.Active,
            IsEnabled = true,
        };

        var result = await _userManager.CreateAsync(admin, password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(admin, RoleNames.Admin);
            _logger.LogInformation("Admin user seeded: {Email}", email);
        }
        else
        {
            _logger.LogError("Failed to seed admin: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }
}
