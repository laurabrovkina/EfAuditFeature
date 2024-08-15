using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EfAuditFeathre.Database;

public class AuditInterceptor : SaveChangesInterceptor
{
    private readonly List<AuditEntity> _auditEntities;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuditInterceptor(List<AuditEntity> auditEntities,
        IPublishEndpoint publishEndpoint)
    {
        _auditEntities = auditEntities;
        _publishEndpoint = publishEndpoint;
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
            await _publishEndpoint.Publish(new AuditTrailMessage
            {
                AuditEntities = _auditEntities
            });
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    public override async Task SaveChangesFailedAsync(DbContextErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
        {
            return;
        }

        var endTime = DateTime.UtcNow;

        foreach (var auditEntity in _auditEntities)
        {
            auditEntity.EndTimeUtc = endTime;
            auditEntity.Succeeded = false;
            auditEntity.ErrorMessage = eventData.Exception.Message;
        }

        if (_auditEntities.Count > 0)
        {
            await _publishEndpoint.Publish(new AuditTrailMessage
            {
                AuditEntities = _auditEntities
            });
        }
    }
}

public record AuditTrailMessage
{
    public List<AuditEntity> AuditEntities { get; set; } = [];
}
