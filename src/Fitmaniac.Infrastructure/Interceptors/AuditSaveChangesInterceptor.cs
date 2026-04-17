using Fitmaniac.Application.Abstractions;
using Fitmaniac.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Fitmaniac.Infrastructure.Interceptors;

public sealed class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;

    public AuditSaveChangesInterceptor(ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken ct = default)
    {
        UpdateAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, ct);
    }

    private void UpdateAuditFields(DbContext? context)
    {
        if (context is null) return;
        var now = DateTime.UtcNow;
        var userId = _currentUser.UserId?.ToString();

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedUtc = now;
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.UpdatedUtc = now;
                    entry.Entity.UpdatedBy = userId;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedUtc = now;
                    entry.Entity.UpdatedBy = userId;
                    break;
            }
        }
    }
}
