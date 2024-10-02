using EfAuditFeathre.Models;

namespace EfAuditFeathre.Services;

public interface IPersonService
{
    Task<IEnumerable<Person>> GetAll();
    Task<Person?> GetById(Guid id);
    Task Create(Person person);
    Task Update(Person person);
    Task<bool> DeleteById(Guid id);
}
