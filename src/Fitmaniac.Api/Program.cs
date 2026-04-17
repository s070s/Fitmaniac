using Fitmaniac.Api.Endpoints;
using Fitmaniac.Api.Hubs;
using Fitmaniac.Api.Middleware;
using Fitmaniac.Infrastructure.DependencyInjection;
using Fitmaniac.Infrastructure.Persistence;
using Fitmaniac.Infrastructure.Seeders;
using Fitmaniac.Shared.Constants;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownIPNetworks.Clear();
    o.KnownProxies.Clear();
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(o =>
    o.AddPolicy("AllowFrontend", p =>
        p.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(PolicyNames.Admin, p => p.RequireRole(RoleNames.Admin))
    .AddPolicy(PolicyNames.Trainer, p => p.RequireRole(RoleNames.Trainer))
    .AddPolicy(PolicyNames.Client, p => p.RequireRole(RoleNames.Client))
    .AddPolicy(PolicyNames.StaffOrOwner, p => p.RequireRole(RoleNames.Admin, RoleNames.Trainer));

builder.Services.AddRateLimiter(o =>
{
    o.AddFixedWindowLimiter("login", c => { c.PermitLimit = 5; c.Window = TimeSpan.FromMinutes(1); c.QueueLimit = 2; });
    o.AddFixedWindowLimiter("refresh", c => { c.PermitLimit = 20; c.Window = TimeSpan.FromMinutes(1); });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();

var app = builder.Build();

// Migrate + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FitmaniacDbContext>();
    await db.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<DatabaseSeederOrchestrator>().SeedAllAsync();
}

app.UseForwardedHeaders();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseMiddleware<DisabledUserMiddleware>();
app.UseAuthorization();
app.UseRateLimiter();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Fitmaniac API — running");

var api = app.MapGroup("/api");
api.MapAuthEndpoints();
api.MapUserProfileEndpoints();
api.MapAdminUserEndpoints();
api.MapTrainerEndpoints();
api.MapAdminTrainerEndpoints();
api.MapClientEndpoints();
api.MapWorkoutEndpoints();
api.MapAdminWorkoutEndpoints();
api.MapExerciseDefinitionEndpoints();
api.MapEquipmentEndpoints();
api.MapWeeklyProgramEndpoints();
api.MapGoalEndpoints();
api.MapMeasurementEndpoints();
api.MapMedicalHistoryEndpoints();
api.MapSubscriptionEndpoints();
api.MapBillingEndpoints();
api.MapProgressEndpoints();
api.MapMediaEndpoints();
api.MapChatEndpoints();

app.MapHub<ChatHub>("/hubs/chat").RequireAuthorization();

app.Run();
