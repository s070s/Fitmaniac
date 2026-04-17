namespace Fitmaniac.Domain.Entities;

public class ExerciseEquipment
{
    public int ExerciseDefinitionId { get; set; }
    public ExerciseDefinition ExerciseDefinition { get; set; } = null!;
    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;
}
