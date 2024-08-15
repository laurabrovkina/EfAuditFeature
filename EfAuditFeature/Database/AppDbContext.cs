using EfAuditFeathre.Models;
using Microsoft.EntityFrameworkCore;

namespace EfAuditFeathre.Database;

public class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }

    public DbSet<Person> People { get; set; }
}
