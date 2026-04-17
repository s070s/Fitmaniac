using Fitmaniac.Domain.Entities;
using Fitmaniac.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Fitmaniac.Infrastructure.Persistence.Configurations;

public sealed class MedicalHistoryConfiguration : IEntityTypeConfiguration<MedicalHistory>
{
    public void Configure(EntityTypeBuilder<MedicalHistory> builder)
    {
        builder.Property(m => m.Conditions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<ICollection<MedicalCondition>>(v, (JsonSerializerOptions?)null) ?? new List<MedicalCondition>());

        builder.Property(m => m.MedicationTypes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<ICollection<MedicationType>>(v, (JsonSerializerOptions?)null) ?? new List<MedicationType>());

        builder.Property(m => m.Surgeries)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<ICollection<SurgeryType>>(v, (JsonSerializerOptions?)null) ?? new List<SurgeryType>());
    }
}
