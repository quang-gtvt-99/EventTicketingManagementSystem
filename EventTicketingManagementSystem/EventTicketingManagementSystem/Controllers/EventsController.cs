using Microsoft.AspNetCore.Mvc;
using EventTicketingManagementSystem.Services.Interfaces;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Dtos;

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

        //User///////
        [HttpGet("event-list")]
        public async Task<IActionResult> GetEventList()
        {
            var result = await _eventService.GetAllEventAsync();
            if (result == null || !result.Any()) return NoContent();
            return Ok(result);
        }

        [HttpGet("event-detail/{id}")]
        public async Task<IActionResult> GetEventDetail(int id)
        {
            var result = await _eventService.GetEventDetailByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("event-booking/{id}")]
        public async Task<IActionResult> GetEventBookingInfo(int id)
        {
            var result = await _eventService.GetEventInfoWithSeatAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpPost("register-seat")]
        public async Task<IActionResult> RegisterSeats([FromBody] CreateSeatDto createSeatDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _eventService.RegisterSeats(createSeatDto);

            return Ok(new { Message = result.Message, TotalSeats = result.TotalSeats });
        }
        [HttpPut("update-seat")]
        public async Task<IActionResult> UpdateSeat([FromBody] UpdateSeatDto updateSeatDto)
        {
            if (updateSeatDto == null)
                return BadRequest("Seat information is required.");

            if (updateSeatDto.SeatId != 0)
            {
                var result = await _eventService.UpdateSeatAsync(updateSeatDto.SeatId, updateSeatDto);
                return result ? Ok("updated successfully.") : NotFound("not found.");
            }
            else if (updateSeatDto.EventId != 0 && !string.IsNullOrEmpty(updateSeatDto.Row) && updateSeatDto.Number != 0)
            {
                var result = await _eventService.UpdateSeatAsync(updateSeatDto.EventId, updateSeatDto.Row, updateSeatDto.Number, updateSeatDto);
                return result ? Ok("updated successfully.") : NotFound("not found.");
            }
            return BadRequest("Invalid input parameters.");
        }

    }
}
