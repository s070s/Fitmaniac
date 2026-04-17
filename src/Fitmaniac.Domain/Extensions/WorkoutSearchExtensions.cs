using Fitmaniac.Domain.Entities;

namespace Fitmaniac.Domain.Extensions;

public static class WorkoutSearchExtensions
{
    public static IQueryable<Workout> InRange(this IQueryable<Workout> query, DateTime? from, DateTime? to)
    {
        if (from.HasValue)
            query = query.Where(w => w.ScheduledDateTime >= from.Value);
        if (to.HasValue)
            query = query.Where(w => w.ScheduledDateTime <= to.Value);
        return query;
    }
}
