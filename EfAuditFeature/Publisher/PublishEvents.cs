using StackExchange.Redis;
using System.Text.Json;
using EfAuditFeathre.Models;

namespace EfAuditFeathre.Publisher;

public class PublishEvents : IPublishEvents
{
    public readonly string _connectionString = "localhost:6379";
    public readonly string _channelName = "messages";

    public async Task PublishMessageAsync(Person person)
    {
        var connection = ConnectionMultiplexer.Connect(_connectionString);
        var subscriber = connection.GetSubscriber();

        var message = new RedisMessage(Guid.NewGuid(), person, DateTime.UtcNow);

        var json = JsonSerializer.Serialize(message);

        await subscriber.PublishAsync(_channelName, json);
    }
}

record RedisMessage(Guid Id, Person person, DateTime Time);