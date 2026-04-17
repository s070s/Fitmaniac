namespace Fitmaniac.Shared.DTOs.Workouts;

public sealed record WorkoutListItemDto(
    int Id,
    DateTime ScheduledDateTime,
    string Type,
    int DurationInMinutes,
    int? TrainerId,
    int? WeeklyProgramId,
    bool CompletedByMe);

public sealed record WorkoutDto(
    int Id,
    DateTime ScheduledDateTime,
    string Type,
    int DurationInMinutes,
    string? Notes,
    int? TrainerId,
    int? WeeklyProgramId,
    IReadOnlyList<WorkoutExerciseDto> Exercises,
    IReadOnlyList<int> ClientIds);

public sealed record CreateWorkoutDto(
    IReadOnlyList<int> ClientIds,
    int? TrainerId,
    int? WeeklyProgramId,
    DateTime ScheduledDateTime,
    string Type,
    int DurationInMinutes,
    string? Notes);

public sealed record UpdateWorkoutDto(
    IReadOnlyList<int>? ClientIds,
    int? TrainerId,
    int? WeeklyProgramId,
    DateTime? ScheduledDateTime,
    string? Type,
    int? DurationInMinutes,
    string? Notes);

public sealed record WorkoutExerciseDto(
    int Id,
    int WorkoutId,
    int ExerciseDefinitionId,
    string ExerciseName,
    IReadOnlyList<WorkoutExerciseSetDto> Sets,
    string? Notes);

public sealed record CreateWorkoutExerciseDto(
    int WorkoutId,
    int ExerciseDefinitionId,
    string? Notes);

public sealed record UpdateWorkoutExerciseDto(
    int Id,
    string? Notes);

public sealed record WorkoutExerciseSetDto(
    int Id,
    int WorkoutExerciseId,
    int SetNumber,
    int Repetitions,
    double? Weight,
    GoalUnit? GoalUnit,
    IntensityLevel OverallIntensityLevel,
    int DurationInSeconds,
    int RestPeriodInSeconds,
    string? Notes);

public sealed record CreateWorkoutExerciseSetDto(
    int WorkoutExerciseId,
    int SetNumber,
    int Repetitions,
    double? Weight,
    GoalUnit? GoalUnit,
    IntensityLevel OverallIntensityLevel,
    int DurationInSeconds,
    int RestPeriodInSeconds,
    string? Notes);

public sealed record UpdateWorkoutExerciseSetDto(
    int Id,
    int? SetNumber,
    int? Repetitions,
    double? Weight,
    GoalUnit? GoalUnit,
    IntensityLevel? OverallIntensityLevel,
    int? DurationInSeconds,
    int? RestPeriodInSeconds,
    string? Notes);
