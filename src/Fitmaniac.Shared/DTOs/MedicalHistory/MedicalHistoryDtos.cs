namespace Fitmaniac.Shared.DTOs.MedicalHistory;

public sealed record MedicalHistoryDto(
    int Id,
    int ClientId,
    string? Description,
    IReadOnlyList<MedicalCondition> Conditions,
    IReadOnlyList<MedicationType> MedicationTypes,
    IReadOnlyList<SurgeryType> Surgeries,
    IntensityLevel? RecommendedIntensityLevel);

public sealed record CreateMedicalHistoryDto(
    int ClientId,
    string? Description,
    IReadOnlyList<MedicalCondition>? Conditions,
    IReadOnlyList<MedicationType>? MedicationTypes,
    IReadOnlyList<SurgeryType>? Surgeries,
    IntensityLevel? RecommendedIntensityLevel);

public sealed record UpdateMedicalHistoryDto(
    string? Description,
    IReadOnlyList<MedicalCondition>? Conditions,
    IReadOnlyList<MedicationType>? MedicationTypes,
    IReadOnlyList<SurgeryType>? Surgeries,
    IntensityLevel? RecommendedIntensityLevel);
