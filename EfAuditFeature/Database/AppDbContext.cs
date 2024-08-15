using EfAuditFeathre.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EfAuditFeathre.Database;

public class AppDbContext : DbContext
{
    private readonly List<AuditEntity> _auditEntities;
    private readonly IPublishEndpoint _publishEndpoint;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        [FromKeyedServices("Audit")] List<AuditEntity> auditEntities,
        IPublishEndpoint publishEndpoint) : base(options)
    {
        _auditEntities = auditEntities;
        _publishEndpoint = publishEndpoint;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db")
            .AddInterceptors(new AuditInterceptor(_auditEntities, _publishEndpoint));
    }

    public DbSet<Person> People { get; set; }
    public DbSet<AuditEntity> AuditEntities { get; set; }
}

public class AuditEntity
{
    [Key]
    public Guid Id { get; set; }
    public string Metadata { get; set; } = string.Empty;
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public bool Succeeded { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
