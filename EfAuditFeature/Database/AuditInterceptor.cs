using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EfAuditFeathre.Database;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly List<AuditEntity> _auditEntities;

    public AuditInterceptor(List<AuditEntity> auditEntities)
    {
        _auditEntities = auditEntities;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if(eventData.Context is null)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        var startTimeUtc = DateTime.UtcNow;

        var auditEntitis = eventData.Context.ChangeTracker.Entries()
            .Where(e => e.Entity is not AuditEntity
                        &&
                        e.State is EntityState.Added or EntityState.Modified or EntityState.Detached)
            .Select(e => new AuditEntity
            {
                Id = Guid.CreateVersion7(),
                Metadata = e.DebugView.LongView,
                StartTimeUtc = startTimeUtc,
            }).ToList();

        if (auditEntitis.Count == 0)
        {
            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        _auditEntities.AddRange(auditEntitis);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        var endTime = DateTime.UtcNow;

        foreach (var auditEntity in _auditEntities)
        {
            auditEntity.EndTimeUtc = endTime;
            auditEntity.Succeeded = true;
        }

        if(_auditEntities.Count > 0)
        {
            eventData.Context.Set<AuditEntity>().AddRange(_auditEntities);
            _auditEntities.Clear();
            await eventData.Context.SaveChangesAsync(cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {

    }
}
