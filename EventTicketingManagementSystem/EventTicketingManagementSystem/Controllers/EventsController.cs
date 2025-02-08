using Microsoft.AspNetCore.Mvc;
using EventTicketingManagementSystem.Services.Interfaces;
using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET: api/events
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEventById(int id)
        {
            var eventItem = await _eventService.GetEventById(id);
            if (eventItem == null) return NotFound();
            return Ok(eventItem);
        }

        // GET: api/events/filter?search=music&category=Concert&status=Active
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByFilter(
            [FromQuery] string search,
            [FromQuery] string category,
            [FromQuery] string status)
        {
            var events = await _eventService.GetEventsByFilter(search, category, status);
            return Ok(events);
        }

        // POST: api/events
        [HttpPost]
        public async Task<ActionResult<int>> CreateEvent([FromBody] Event eventItem)
        {
            var newEvent = await _eventService.CreateEvent(eventItem);
            return CreatedAtAction(nameof(GetEventById), newEvent);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event eventItem)
        {
            if (id != eventItem.Id) return BadRequest();

            var updated = await _eventService.UpdateEvent(eventItem);
            if (!updated) return NotFound();

            return NoContent();
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id, [FromBody] Event eventItem)
        {
            var deleted = await _eventService.DeleteEvent(eventItem);

            if (!deleted) return NotFound();

            return NoContent();
        }

    }
}
