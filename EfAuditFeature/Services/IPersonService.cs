using EfAuditFeathre.Models;

namespace EfAuditFeathre.Services;

public interface IPersonService
{
    IEnumerable<Person> GetAll();
    Person GetById(Guid id);
    Task Add(Person person);
    Task Update(Person person);
    Task Delete(Guid id);
}
