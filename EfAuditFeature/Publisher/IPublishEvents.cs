using EfAuditFeathre.Models;

namespace EfAuditFeathre.Publisher;

public interface IPublishEvents
{
    Task PublishMessageAsync(Person person);
}