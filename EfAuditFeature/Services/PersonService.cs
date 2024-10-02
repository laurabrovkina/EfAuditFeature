using EfAuditFeathre.Database;
using EfAuditFeathre.Models;
using Microsoft.EntityFrameworkCore;

namespace EfAuditFeathre.Services;

public class PersonService : IPersonService
{
    private readonly AppDbContext _dbContext;

    public PersonService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Person>> GetAll()
    {
        //return await _dbContext.People.ToListAsync();

        // Returns all records from db, soft deleted and active
        return await _dbContext.People.IgnoreQueryFilters().ToListAsync();
    }

    public async Task<Person?> GetById(Guid id)
    {
        return await _dbContext.People.FindAsync(id);
    }

    public async Task Create(Person person)
    {
        _dbContext.People.Add(person);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(Person person)
    {
        _dbContext.People.Update(person);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> DeleteById(Guid id)
    {
        var result = await _dbContext.People
            .Where(p => p.Id == id && !p.IsDeleted)
            .ExecuteUpdateAsync(x => x
                .SetProperty(s => s.IsDeleted, true)
                .SetProperty(s => s.DeletedAtUtc, DateTime.UtcNow));

        return result > 0;
    }
}
