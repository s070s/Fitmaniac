using Fitmaniac.MAUI.Pages;
using Fitmaniac.MAUI.Services;
using Fitmaniac.MAUI.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Fitmaniac.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configuration from embedded appsettings.json
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("Fitmaniac.MAUI.appsettings.json");
        if (stream is not null)
        {
            var config = new ConfigurationBuilder().AddJsonStream(stream).Build();
            builder.Configuration.AddConfiguration(config);
        }

        var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://10.0.2.2:7301";

        // Auth token store
        builder.Services.AddSingleton<IAuthTokenStore, AuthTokenStore>();

        // HTTP client with auth handler
        builder.Services.AddTransient<AuthHttpHandler>();
        builder.Services.AddHttpClient<IApiClient, ApiClient>(c =>
        {
            c.BaseAddress = new Uri(apiBaseUrl);
        })
        .AddHttpMessageHandler<AuthHttpHandler>();

        // Services
        builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddTransient<IWorkoutService, WorkoutService>();
        builder.Services.AddTransient<IExerciseService, ExerciseService>();
        builder.Services.AddTransient<IProgramService, ProgramService>();
        builder.Services.AddTransient<IGoalService, GoalService>();
        builder.Services.AddTransient<IMeasurementService, MeasurementService>();
        builder.Services.AddTransient<IProgressService, ProgressService>();

        // ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<WorkoutsViewModel>();
        builder.Services.AddTransient<WorkoutDetailViewModel>();
        builder.Services.AddTransient<ProgressViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<WorkoutsPage>();
        builder.Services.AddTransient<WorkoutDetailPage>();
        builder.Services.AddTransient<ProgressPage>();
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
