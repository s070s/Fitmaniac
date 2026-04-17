namespace Fitmaniac.Shared.DTOs.Exercises;

public sealed record ExerciseDefinitionDto(
    int Id,
    string Name,
    string? Description,
    string? VideoUrl,
    int CaloriesBurnedPerHour,
    bool IsCompoundExercise,
    IReadOnlyList<MuscleGroup> PrimaryMuscleGroups,
    IReadOnlyList<MuscleGroup> SecondaryMuscleGroups,
    ClientExperience? ExperienceLevel,
    string? Category,
    IReadOnlyList<EquipmentDto> Equipments);

public sealed record CreateExerciseDefinitionDto(
    string Name,
    string? Description,
    string? VideoUrl,
    int CaloriesBurnedPerHour,
    bool IsCompoundExercise,
    IReadOnlyList<MuscleGroup> PrimaryMuscleGroups,
    IReadOnlyList<MuscleGroup>? SecondaryMuscleGroups,
    ClientExperience? ExperienceLevel,
    string? Category,
    IReadOnlyList<int>? EquipmentIds);

public sealed record UpdateExerciseDefinitionDto(
    int Id,
    string? Name,
    string? Description,
    string? VideoUrl,
    int? CaloriesBurnedPerHour,
    bool? IsCompoundExercise,
    IReadOnlyList<MuscleGroup>? PrimaryMuscleGroups,
    IReadOnlyList<MuscleGroup>? SecondaryMuscleGroups,
    ClientExperience? ExperienceLevel,
    string? Category,
    IReadOnlyList<int>? EquipmentIds);

public sealed record EquipmentDto(int Id, string Name, string? Description);

public sealed record CreateEquipmentDto(string Name, string? Description);

public sealed record UpdateEquipmentDto(int Id, string? Name, string? Description);
