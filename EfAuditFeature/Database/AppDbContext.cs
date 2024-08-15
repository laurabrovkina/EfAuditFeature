using EfAuditFeathre.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EfAuditFeathre.Database;

public class AppDbContext : DbContext
{
    private readonly List<AuditEntity> _auditEntities;

    public AppDbContext(DbContextOptions<AppDbContext> options,
        List<AuditEntity> auditEntities) : base(options)
    {
        _auditEntities = auditEntities;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db")
            .AddInterceptors(new AuditInterceptor(_auditEntities));
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
