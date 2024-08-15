using EfAuditFeathre.Models;

namespace EfAuditFeathre.Services;

public interface IPersonService
{
    IEnumerable<Person> GetAll();

    // Get a person by their ID
    Person GetById(Guid id);

    // Add a new person
    Task Add(Person person);

    // Update an existing person
    Task Update(Person person);

    // Delete a person by their ID
    Task Delete(Guid id);
}
