using EfAuditFeathre.Database;
using EfAuditFeathre.Models;

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

    public async Task Delete(Guid id)
    {
        var person = GetById(id)
            ?? throw new Exception("Person has not found,");
        
        if (person != null)
        {
            _dbContext.People.Remove(person);
            await _dbContext.SaveChangesAsync();
        }
    }
}
