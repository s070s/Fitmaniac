using System.ComponentModel.DataAnnotations;
using Fitmaniac.Domain.Common;
using Fitmaniac.Domain.Enums;

namespace Fitmaniac.Domain.Entities;

public class MedicalHistory : AuditableEntity
{
    [StringLength(1000)] public string? Description { get; set; }
    public ICollection<MedicalCondition> Conditions { get; set; } = new List<MedicalCondition>();
    public ICollection<MedicationType> MedicationTypes { get; set; } = new List<MedicationType>();
    public ICollection<SurgeryType> Surgeries { get; set; } = new List<SurgeryType>();
    public IntensityLevel? RecommendedIntensityLevel { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}
