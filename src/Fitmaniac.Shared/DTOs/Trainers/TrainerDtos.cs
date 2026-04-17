namespace Fitmaniac.Shared.DTOs.Trainers;

public sealed record TrainerDto(
    int Id,
    int UserId,
    string? FirstName,
    string? LastName,
    string? ProfilePhotoUrl,
    string? Bio,
    IReadOnlyList<TrainerSpecialization> Specializations,
    int? Age,
    string? City,
    string? Country);

public sealed record TrainerListItemDto(
    int Id,
    int UserId,
    string? FirstName,
    string? LastName,
    string? ProfilePhotoUrl,
    IReadOnlyList<TrainerSpecialization> Specializations,
    string? City);

public sealed record CreateTrainerDto(
    int UserId,
    string? FirstName,
    string? LastName,
    string? Bio,
    IReadOnlyList<TrainerSpecialization>? Specializations);

public sealed record UpdateTrainerProfileDto(
    string? FirstName,
    string? LastName,
    string? Bio,
    string? PhoneNumber,
    string? Address,
    string? City,
    string? State,
    string? ZipCode,
    string? Country,
    double? Weight,
    double? Height,
    DateTime? DateOfBirth,
    IReadOnlyList<TrainerSpecialization>? Specializations);
