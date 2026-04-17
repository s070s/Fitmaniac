namespace Fitmaniac.Shared.DTOs.Clients;

public sealed record ClientDto(
    int Id,
    int UserId,
    string? FirstName,
    string? LastName,
    string? ProfilePhotoUrl,
    string? Bio,
    ClientExperience ExperienceLevel,
    IntensityLevel PreferredIntensityLevel,
    int? Age,
    double? Weight,
    double? Height,
    double? BMI,
    double? BMR,
    string? City,
    string? Country);

public sealed record CreateClientDto(
    int UserId,
    string? FirstName,
    string? LastName,
    string? Bio,
    ClientExperience ExperienceLevel = ClientExperience.Beginner,
    IntensityLevel PreferredIntensityLevel = IntensityLevel.Medium);

public sealed record UpdateClientProfileDto(
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
    ClientExperience? ExperienceLevel,
    IntensityLevel? PreferredIntensityLevel);
