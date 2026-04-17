using Fitmaniac.Web.Components;
using Fitmaniac.Web.Consumers;
using Fitmaniac.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// DataProtection
var dpPath = builder.Configuration["DataProtection:KeyPath"] ?? "DataProtection-Keys";
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(dpPath));

// Auth — cookie for Blazor SSR
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opts =>
    {
        opts.Cookie.Name = ".FMC.Auth";
        opts.Cookie.HttpOnly = true;
        opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        opts.Cookie.SameSite = SameSiteMode.Strict;
        opts.LoginPath = "/login";
        opts.LogoutPath = "/logout";
        opts.AccessDeniedPath = "/access-denied";
        opts.SlidingExpiration = true;
        opts.ExpireTimeSpan = TimeSpan.FromDays(7);
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Razor + Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HttpClient consumers pointing at the API
var apiBase = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7301";
builder.Services.AddScoped<SsrApiHandler>();

void AddApiClient<TInterface, TImpl>(IServiceCollection s) where TInterface : class where TImpl : class, TInterface
{
    s.AddHttpClient<TInterface, TImpl>(c => c.BaseAddress = new Uri(apiBase))
     .AddHttpMessageHandler<SsrApiHandler>();
}

AddApiClient<IAuthConsumer, AuthConsumer>(builder.Services);
AddApiClient<IUserConsumer, UserConsumer>(builder.Services);
AddApiClient<ITrainerConsumer, TrainerConsumer>(builder.Services);
AddApiClient<IClientConsumer, ClientConsumer>(builder.Services);
AddApiClient<IWorkoutConsumer, WorkoutConsumer>(builder.Services);
AddApiClient<IExerciseConsumer, ExerciseConsumer>(builder.Services);
AddApiClient<IProgramConsumer, ProgramConsumer>(builder.Services);
AddApiClient<IGoalConsumer, GoalConsumer>(builder.Services);
AddApiClient<IMeasurementConsumer, MeasurementConsumer>(builder.Services);
AddApiClient<IMedicalHistoryConsumer, MedicalHistoryConsumer>(builder.Services);
AddApiClient<ISubscriptionConsumer, SubscriptionConsumer>(builder.Services);
AddApiClient<IBillingConsumer, BillingConsumer>(builder.Services);
AddApiClient<IProgressConsumer, ProgressConsumer>(builder.Services);
AddApiClient<IChatConsumer, ChatConsumer>(builder.Services);

// Auth + SignalR services
builder.Services.AddScoped<WebAuthService>();
builder.Services.AddScoped<ChatSignalRService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Auth helper endpoints
app.MapPost("/auth/signout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).AllowAnonymous();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
