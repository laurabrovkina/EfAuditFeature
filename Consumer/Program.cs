using System.Text.Json;
using EfAuditFeathre.Models;
using StackExchange.Redis;

var connectionString = "localhost:6379";
var channelName = "messages";
//var eventsChannelName = "__keyevent@0__:*";
var connection = ConnectionMultiplexer.Connect(connectionString);

var subscriber = connection.GetSubscriber();

Console.WriteLine($"Consumer started listening for messages in channel {channelName}");

var db = connection.GetDatabase();

await subscriber.SubscribeAsync(channelName, (channel, incomingMessage) =>
{
    //var message = JsonSerializer.Deserialize<Person>(incomingMessage!);

    Console.WriteLine($"Received message from channel {channel}: {incomingMessage}");
});

//await subscriber.SubscribeAsync(eventsChannelName, (channel, incomingMessage) =>
//{
//    Console.WriteLine($"Received message from channel {channel}: {incomingMessage}");
//});

//await db.StringSetAsync("ping", "pong", TimeSpan.FromSeconds(5));

Console.ReadKey();