namespace Fitmaniac.Application.Abstractions;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly TodayUtc { get; }
}
