using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EventTicketingManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/event")]
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

        // GET: api/events/filter?search=music&category=Concert&status=Active&pageNumber=1&pageSize=10
        [HttpGet("filter")]
        [EnableRateLimiting(RateLimitConst.FixedRateLimit)]
        public async Task<IActionResult> GetEventsByFilter([FromQuery] EventSearchParamsRequest filterRequest)
        {
            var events = await _eventService.GetFilteredPagedEventsAsync(filterRequest);

            if (events == null || !events.Any())
                return NoContent();

            var eventDtos = events.Select(e => new EventInfoDto
            {
                EventID = e.Id,
                EventName = e.Name,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                VenueName = e.VenueName,
                VenueAddress = e.VenueAddress,
                ImageUrls = e.ImageUrls
            }).ToList();

            return Ok(eventDtos);
        }


        // POST: api/events
        [HttpPost]
        [Authorize(Roles = $"{RoleConsts.Admin}")]
        public async Task<ActionResult<int>> CreateEvent([FromForm] AddUpdateEventRequest eventItem)
        {
            var newEventID = await _eventService.CreateEvent(eventItem);
            return Ok(newEventID);
        }

        // PUT: api/events/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = $"{RoleConsts.Admin}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromForm] AddUpdateEventRequest eventItem)
        {
            if (id != eventItem.ID) return BadRequest();

            var updated = await _eventService.UpdateEvent(eventItem);
            if (!updated) return NotFound();

            return Ok(true);
        }

        // DELETE: api/events/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = $"{RoleConsts.Admin}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var deleted = await _eventService.DeleteEvent(id);

            if (!deleted) return NotFound();

            return Ok(true);
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
        // [EnableRateLimiting(RateLimitConst.FixedRateLimit)]
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

            return Ok(new { result.Message, result.TotalSeats });
        }
    }
}
