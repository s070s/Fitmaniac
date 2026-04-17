namespace Fitmaniac.Domain.Entities;

public class TrainerClient
{
    public int TrainerId { get; set; }
    public Trainer Trainer { get; set; } = null!;
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public DateTime SubscribedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UnsubscribedUtc { get; set; }
}
