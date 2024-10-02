using Microsoft.AspNetCore.Mvc;
using EfAuditFeathre.Services;
using EfAuditFeathre.Models;
using EfAuditFeathre.Publisher;
using static MassTransit.ValidationResultExtensions;

namespace EfAuditFeathre.Controllers;

[Route("people")]
[ApiController]
public class PeopleController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly IPublishEvents _publishEvents;

    public PeopleController(IPersonService personService,
        IPublishEvents publishEvents)
    {
        _personService = personService;
        _publishEvents = publishEvents;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var people = await _personService.GetAll();
        return Ok(people);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var person = await _personService.GetById(id);
        return person is not null ? Ok(person) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Person person)
    {
        await _personService.Create(person);
        
        await _publishEvents.PublishMessageAsync(person);

        return CreatedAtAction(nameof(Get), new { id = person.Id }, person);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] Person person)
    {
        var existingPerson = _personService.GetById(id);
        if (existingPerson == null)
        {
            return NotFound();
        }
        await _personService.Update(person);
        return Ok(person);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _personService.DeleteById(id);
        return deleted ? Ok() : NotFound();
    }
}