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

    public IEnumerable<Person> GetAll()
    {
        return _dbContext.People.ToList();
    }

    public Person GetById(Guid id)
    {
        return _dbContext.People.FirstOrDefault(p => p.Id == id) 
            ?? throw new Exception("Person has not found,");
    }

    public async Task Add(Person person)
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
