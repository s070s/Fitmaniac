using Fitmaniac.Application.Abstractions;
using Fitmaniac.Application.Data;
using Fitmaniac.Application.Mapping;
using Fitmaniac.Application.Services;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Infrastructure.Identity;
using Fitmaniac.Infrastructure.Interceptors;
using Fitmaniac.Infrastructure.Persistence;
using Fitmaniac.Infrastructure.Seeders;
using Fitmaniac.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Fitmaniac.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // DbContext
        services.AddScoped<AuditSaveChangesInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();
        services.AddDbContext<FitmaniacDbContext>((sp, options) =>
        {
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")!,
                sql => sql.MigrationsAssembly(typeof(FitmaniacDbContext).Assembly.FullName));
        });
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<FitmaniacDbContext>());

        // Identity
        services.AddIdentity<ApplicationUser, IdentityRole<int>>(opts =>
        {
            opts.Password.RequireDigit = true;
            opts.Password.RequiredLength = 8;
            opts.Password.RequireNonAlphanumeric = false;
            opts.User.RequireUniqueEmail = true;
            opts.SignIn.RequireConfirmedEmail = true;
        })
        .AddEntityFrameworkStores<FitmaniacDbContext>()
        .AddDefaultTokenProviders();

        // JWT
        services.AddAuthentication(opts =>
        {
            opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opts =>
        {
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)),
                ClockSkew = TimeSpan.Zero,
            };

            // Allow SignalR tokens from query string
            opts.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    var token = ctx.Request.Query["access_token"];
                    var path = ctx.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(token) && path.StartsWithSegments("/hubs"))
                        ctx.Token = token;
                    return Task.CompletedTask;
                }
            };
        });

        // Infrastructure services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IRefreshTokenCookieService, RefreshTokenCookieService>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IEmailService, StubEmailService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IValidationService, ValidationService>();
        services.AddScoped<IModelToDtoMapper, ModelToDtoMapper>();

        // Application services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<ITrainerService, TrainerService>();
        services.AddScoped<IAdminTrainerService, AdminTrainerService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IWorkoutService, WorkoutService>();
        services.AddScoped<IAdminWorkoutService, AdminWorkoutService>();
        services.AddScoped<IExerciseDefinitionService, ExerciseDefinitionService>();
        services.AddScoped<IEquipmentService, EquipmentService>();
        services.AddScoped<IWeeklyProgramService, WeeklyProgramService>();
        services.AddScoped<IGoalService, GoalService>();
        services.AddScoped<IMeasurementService, MeasurementService>();
        services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
        services.AddScoped<ISubscriptionService, SubscriptionService>();
        services.AddScoped<IBillingService, BillingService>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IChatQueryService, ChatQueryService>();
        services.AddScoped<IProgramPdfService, ProgramPdfService>();

        // Seeders
        services.AddScoped<IdentityRoleSeeder>();
        services.AddScoped<AdminUserSeeder>();
        services.AddScoped<SubscriptionPlanSeeder>();
        services.AddScoped<EquipmentSeeder>();
        services.AddScoped<ExerciseDefinitionSeeder>();
        services.AddScoped<DatabaseSeederOrchestrator>();

        return services;
    }
}
