# EF Audit Feature with AWS SQS
Using MassTransit
```
builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingAmazonSqs((context, cfg) =>
    {
        cfg.Host("us-east-1", _ => { });
        cfg.ConfigureEndpoints(context);
    });
});
```


## Redis pub-sub

* Add `StackExchange.Redis` nuget package
* Set connection string and name for the channel. They should match for the publisher and subscriber
* To enable feature that will track key events go to Redis CLI and execute the command:
```
config set notify-keyspace-events KEA
```
* Use specific channel convention
```
var channelName = "__keyevent@0__:*";
```
* It could be for all events or specific ones: `expired`, `expire`, etc.
* This example is commented out for now in Consumer
