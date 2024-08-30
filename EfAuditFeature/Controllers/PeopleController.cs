using Microsoft.AspNetCore.Mvc;
using EfAuditFeathre.Services;
using EfAuditFeathre.Models;
using EfAuditFeathre.Publisher;

namespace EfAuditFeathre.Controllers
{
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

        [HttpGet("people")]
        public IActionResult GetPeople()
        {
            var people = _personService.GetAll();
            return Ok(people);
        }

        [HttpGet("people/{id}")]
        public IActionResult GetPerson(Guid id)
        {
            var person = _personService.GetById(id);
            if (person == null)
            {
                return NotFound();
            }
            return Ok(person);
        }

        [HttpPost("people")]
        public IActionResult AddPerson([FromBody] Person person)
        {
            _personService.Add(person);

            _publishEvents.PublishMessageAsync(person);

            return Ok();
        }

        [HttpPut("people/{id}")]
        public IActionResult UpdatePerson(Guid id, [FromBody] Person person)
        {
            var existingPerson = _personService.GetById(id);
            if (existingPerson == null)
            {
                return NotFound();
            }
            _personService.Update(person);
            return Ok();
        }

        [HttpDelete("people/{id}")]
        public IActionResult DeletePerson(Guid id)
        {
            var existingPerson = _personService.GetById(id);
            if (existingPerson == null)
            {
                return NotFound();
            }
            _personService.Delete(id);
            return Ok();
        }
    }
}