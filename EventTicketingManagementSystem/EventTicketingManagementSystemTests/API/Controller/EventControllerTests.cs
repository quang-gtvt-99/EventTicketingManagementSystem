using EventTicketingManagementSystem.API.Controllers;
using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EventTicketingManagementSystemTests.API.Controller
{
    public class EventControllerTests
    {
        private readonly Mock<IEventService> _mockEventService;
        private readonly EventsController _eventsController;

        public EventControllerTests()
        {
            _mockEventService = new Mock<IEventService>();
            _eventsController = new EventsController(_mockEventService.Object);
        }

        [Fact]
        public async Task GetAllEvents_ShouldReturnOkResult_WhenEventsExist()
        {
            // Arrange
            var events = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1" },
                new Event { Id = 2, Name = "Event 2" }
            };
            _mockEventService.Setup(s => s.GetAllEventsAsync()).ReturnsAsync(events);

            // Act
            var result = await _eventsController.GetAllEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvents = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Equal(2, returnedEvents.Count);
        }

        [Fact]
        public async Task GetEventById_ShouldReturnOkResult_WhenEventExists()
        {
            // Arrange
            var eventId = 1;
            var eventItem = new Event { Id = eventId, Name = "Test Event" };
            _mockEventService.Setup(s => s.GetEventById(eventId)).ReturnsAsync(eventItem);

            // Act
            var result = await _eventsController.GetEventById(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEvent = Assert.IsType<Event>(okResult.Value);
            Assert.Equal(eventId, returnedEvent.Id);
        }

        [Fact]
        public async Task GetEventById_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            var eventId = 999;
            _mockEventService.Setup(s => s.GetEventById(eventId)).ReturnsAsync((Event?)null);

            // Act
            var result = await _eventsController.GetEventById(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetEventsByFilter_ShouldReturnOkResult_WhenEventsFound()
        {
            // Arrange
            var filterRequest = new EventSearchParamsRequest
            {
                Search = "test",
                Category = "music",
                Status = "active"
            };

            var events = new List<Event>
            {
                new Event {
                    Id = 1,
                    Name = "Test Event",
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1),
                    VenueName = "Test Venue",
                    VenueAddress = "Test Address"
                }
            };

            _mockEventService.Setup(s => s.GetFilteredPagedEventsAsync(filterRequest))
                .ReturnsAsync(events);

            // Act
            var result = await _eventsController.GetEventsByFilter(filterRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEvents = Assert.IsType<List<EventInfoDto>>(okResult.Value);
            Assert.Single(returnedEvents);
        }

        [Fact]
        public async Task CreateEvent_ShouldReturnOkResult_WhenEventCreated()
        {
            // Arrange
            var eventRequest = new AddUpdateEventRequest
            {
                Name = "New Event",
                Description = "Test Description"
            };
            var newEventId = 1;
            _mockEventService.Setup(s => s.CreateEvent(eventRequest)).ReturnsAsync(newEventId);

            // Act
            var result = await _eventsController.CreateEvent(eventRequest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(newEventId, okResult.Value);
        }

        [Fact]
        public async Task UpdateEvent_ShouldReturnOkResult_WhenEventUpdated()
        {
            // Arrange
            var eventId = 1;
            var eventRequest = new AddUpdateEventRequest
            {
                ID = eventId,
                Name = "Updated Event"
            };
            _mockEventService.Setup(s => s.UpdateEvent(eventRequest)).ReturnsAsync(true);

            // Act
            var result = await _eventsController.UpdateEvent(eventId, eventRequest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateEvent_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            var eventId = 1;
            var eventRequest = new AddUpdateEventRequest { ID = 2 };

            // Act
            var result = await _eventsController.UpdateEvent(eventId, eventRequest);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task RegisterSeats_ShouldReturnOkResult_WhenValidRequest()
        {
            // Arrange
            var createSeatDto = new CreateSeatDto
            {
                EventId = 1,
                Price = 100
            };
            var expectedMessage = "registed successfully.";
            var expectedTotalSeats = 160; // 10 rows (A-J) x 16 seats per row

            _mockEventService.Setup(s => s.RegisterSeats(createSeatDto))
                .ReturnsAsync((expectedMessage, expectedTotalSeats));

            // Act
            var result = await _eventsController.RegisterSeats(createSeatDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEvent_ShouldReturnOkResult_WhenEventDeleted()
        {
            // Arrange
            var eventId = 1;
            _mockEventService.Setup(s => s.DeleteEvent(eventId)).ReturnsAsync(true);

            // Act
            var result = await _eventsController.DeleteEvent(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task DeleteEvent_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            var eventId = 999;
            _mockEventService.Setup(s => s.DeleteEvent(eventId)).ReturnsAsync(false);

            // Act
            var result = await _eventsController.DeleteEvent(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetEventList_ShouldReturnOkResult_WhenEventsExist()
        {
            // Arrange
            var events = new List<Event>
    {
        new Event { Id = 1, Name = "Event 1" },
        new Event { Id = 2, Name = "Event 2" }
    };
            _mockEventService.Setup(s => s.GetAllEventAsync()).ReturnsAsync(events);

            // Act
            var result = await _eventsController.GetEventList();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEvents = Assert.IsType<List<Event>>(okResult.Value);
            Assert.Equal(2, returnedEvents.Count);
        }

        [Fact]
        public async Task GetEventList_ShouldReturnNoContent_WhenNoEventsExist()
        {
            // Arrange
            _mockEventService.Setup(s => s.GetAllEventAsync()).ReturnsAsync(new List<Event>());

            // Act
            var result = await _eventsController.GetEventList();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task GetEventDetail_ShouldReturnOkResult_WhenEventExists()
        {
            // Arrange
            var eventId = 1;
            var eventItem = new Event { Id = eventId, Name = "Test Event" };
            _mockEventService.Setup(s => s.GetEventDetailByIdAsync(eventId)).ReturnsAsync(eventItem);

            // Act
            var result = await _eventsController.GetEventDetail(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedEvent = Assert.IsType<Event>(okResult.Value);
            Assert.Equal(eventId, returnedEvent.Id);
        }

        [Fact]
        public async Task GetEventDetail_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            var eventId = 999;
            _mockEventService.Setup(s => s.GetEventDetailByIdAsync(eventId)).ReturnsAsync((Event?)null);

            // Act
            var result = await _eventsController.GetEventDetail(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetEventBookingInfo_ShouldReturnOkResult_WhenEventExists()
        {
            // Arrange
            var eventId = 1;
            var eventBookingInfo = new EventBookingInfoDto
            {
                EventInfo = new EventInfoDto
                {
                    EventID = eventId,
                    EventName = "Test Event"
                },
                SeatInfos = new List<SeatInfoDto>()
            };

            _mockEventService.Setup(s => s.GetEventInfoWithSeatAsync(eventId))
                .ReturnsAsync(eventBookingInfo);

            // Act
            var result = await _eventsController.GetEventBookingInfo(eventId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedInfo = Assert.IsType<EventBookingInfoDto>(okResult.Value);
            Assert.Equal(eventId, returnedInfo.EventInfo.EventID);
        }

        [Fact]
        public async Task GetEventBookingInfo_ShouldReturnNotFound_WhenEventDoesNotExist()
        {
            // Arrange
            var eventId = 999;
            _mockEventService.Setup(s => s.GetEventInfoWithSeatAsync(eventId)).ReturnsAsync((EventBookingInfoDto?) null);

            // Act
            var result = await _eventsController.GetEventBookingInfo(eventId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}