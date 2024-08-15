using EfAuditFeathre.Database;
using EfAuditFeathre.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddKeyedScoped<List<AuditEntity>>("Audit", (_,_) => new());

builder.Services.AddScoped<IPersonService, PersonService>();

builder.Services.AddMassTransit(x =>
{
    x.UsingAmazonSqs((context, cfg) =>
    {
        cfg.Host("us-east-1", _ => { });
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();
