using Fitmaniac.Application.Data;
using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace Fitmaniac.Infrastructure.Seeders;

public sealed class ExerciseDefinitionSeeder
{
    private readonly IApplicationDbContext _db;
    private readonly ILogger<ExerciseDefinitionSeeder> _logger;

    public ExerciseDefinitionSeeder(IApplicationDbContext db, ILogger<ExerciseDefinitionSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        if (await _db.ExerciseDefinitions.AnyAsync()) return;

        var files = new[]
        {
            "Fitmaniac.Shared.JSON.Exercises.exercises.core.json",
            "Fitmaniac.Shared.JSON.Exercises.exercises.strength.json",
            "Fitmaniac.Shared.JSON.Exercises.exercises.cardio.json",
        };

        var assembly = Assembly.Load("Fitmaniac.Shared");
        int total = 0;

        foreach (var file in files)
        {
            using var stream = assembly.GetManifestResourceStream(file);
            if (stream is null) continue;
            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();

            var items = JsonSerializer.Deserialize<List<ExerciseSeedItem>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (items is null) continue;

            foreach (var item in items)
            {
                _db.ExerciseDefinitions.Add(new ExerciseDefinition
                {
                    Name = item.Name,
                    Description = item.Description,
                    VideoUrl = item.VideoUrl,
                    CaloriesBurnedPerHour = item.CaloriesBurnedPerHour,
                    IsCompoundExercise = item.IsCompoundExercise,
                    PrimaryMuscleGroups = item.PrimaryMuscleGroups?.Select(Enum.Parse<MuscleGroup>).ToList() ?? [],
                    SecondaryMuscleGroups = item.SecondaryMuscleGroups?.Select(Enum.Parse<MuscleGroup>).ToList() ?? [],
                    ExperienceLevel = item.ExperienceLevel is null ? null : Enum.Parse<ClientExperience>(item.ExperienceLevel),
                    Category = item.Category,
                });
                total++;
            }
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("ExerciseDefinitions seeded: {Count} items.", total);
    }

    private sealed record ExerciseSeedItem(
        string Name,
        string? Description,
        string? VideoUrl,
        int CaloriesBurnedPerHour,
        bool IsCompoundExercise,
        List<string>? PrimaryMuscleGroups,
        List<string>? SecondaryMuscleGroups,
        string? ExperienceLevel,
        string? Category);
}
