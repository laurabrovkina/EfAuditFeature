using EfAuditFeathre.Database;
using EfAuditFeathre.Publisher;
using EfAuditFeathre.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddKeyedScoped<List<AuditEntity>>("Audit", (_,_) => new());

builder.Services.AddScoped<IPersonService, PersonService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumers(typeof(Program).Assembly);
    x.UsingAmazonSqs((context, cfg) =>
    {
        cfg.Host("us-east-1", _ => { });
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<IPublishEvents, PublishEvents>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
