namespace Fitmaniac.Shared.DTOs.Measurements;

public sealed record MeasurementDto(
    int Id,
    int ClientId,
    GoalUnit Unit,
    double Value,
    DateTime Date,
    IntensityLevel Intensity,
    bool IsPersonalBest,
    string? Notes);

public sealed record CreateMeasurementDto(
    int ClientId,
    GoalUnit Unit,
    double Value,
    DateTime Date,
    IntensityLevel Intensity,
    string? Notes);

public sealed record UpdateMeasurementDto(
    int Id,
    GoalUnit? Unit,
    double? Value,
    DateTime? Date,
    IntensityLevel? Intensity,
    string? Notes);
