using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;

namespace Fitmaniac.Domain.Entities;

public abstract class PersonalInformation : AuditableEntity
{
    [Url] public string? ProfilePhotoUrl { get; set; }
    [MaxLength(30)] public string? FirstName { get; set; }
    [MaxLength(30)] public string? LastName { get; set; }
    [DataType(DataType.Date)] public DateTime? DateOfBirth { get; set; }
    [Phone, MaxLength(20)] public string? PhoneNumber { get; set; }
    [MaxLength(100)] public string? Address { get; set; }
    [MaxLength(50)] public string? City { get; set; }
    [MaxLength(50)] public string? State { get; set; }
    [MaxLength(10)] public string? ZipCode { get; set; }
    [MaxLength(50)] public string? Country { get; set; }
    [Range(0, 500)] public double? Weight { get; set; }
    [Range(0, 300)] public double? Height { get; set; }
    [Range(0, 10000)] public double? BMR { get; set; }
    [Range(0, 100)] public double? BMI { get; set; }

    public int? Age => DateOfBirth is null ? null : CalculateAge(DateOfBirth.Value);

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.UtcNow;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
}
