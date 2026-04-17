namespace Fitmaniac.Domain.Entities;

public class ClientWorkout
{
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;
    public DateTime? CompletedUtc { get; set; }
    public int? PerceivedIntensity { get; set; }
    public string? ClientNotes { get; set; }
}
