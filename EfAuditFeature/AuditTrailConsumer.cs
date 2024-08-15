using EfAuditFeathre.Database;
using MassTransit;

namespace EfAuditFeathre;

public class AuditTrailConsumer : IConsumer<AuditTrailMessage>
{
    private readonly AppDbContext _dbContext;

    public AuditTrailConsumer(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<AuditTrailMessage> context)
    {
        _dbContext.AuditEntities.AddRange(context.Message.AuditEntities);
        await _dbContext.SaveChangesAsync();
    }
}
