using Fitmaniac.Domain.Entities;

namespace Fitmaniac.Domain.Extensions;

public static class UserSearchExtensions
{
    public static IQueryable<ApplicationUser> SearchByTerm(this IQueryable<ApplicationUser> query, string? term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return query;

        var lower = term.Trim().ToLower();
        return query.Where(u =>
            u.Email!.ToLower().Contains(lower) ||
            u.UserName!.ToLower().Contains(lower));
    }
}
