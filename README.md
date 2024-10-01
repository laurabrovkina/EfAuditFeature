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

## Soft Delete
Updated `PeopleController` with a new implementation of `Delete` endpoint.
New migration was run and db was updated:
```
dotnet ef migrations add AddSoftDelete
dotnet ef database update
```
The idea is to set a deleted entity flag to `true` when we remove that one from db. It will allow us to restore data in case we need it later:
```
        var result = await _dbContext.People
            .Where(p => p.Id == id && !p.IsDeleted)
            .ExecuteUpdateAsync(x => x
                .SetProperty(s => s.IsDeleted, true)
                .SetProperty(s => s.DeletedAtUtc, DateTime.UtcNow));
```
Also, we need to track the time when the soft delete took place.
After that change instead of updating every endpoint, the filter on db context could be set to ignore marked as deleted entities when reading from db. We use EF feature to implement this functionality:
```
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasQueryFilter(x => !x.IsDeleted);
    }
```
**Note**: Implementation of the `PeopleController` requires refactoring with using DTO models and adjust logic for each endpoint.