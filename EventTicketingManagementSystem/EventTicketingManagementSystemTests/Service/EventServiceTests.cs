using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Services.Services.Implements;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace EventTicketingManagementSystemTests.Service
{
    public class EventServiceTests
    {
        private readonly Mock<IEventRepository> _mockEventRepository;
        private readonly Mock<IObjectStorageService> _mockObjectStorageService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly EventService _eventService;

        public EventServiceTests()
        {
            _mockEventRepository = new Mock<IEventRepository>();
            _mockObjectStorageService = new Mock<IObjectStorageService>();
            _mockCacheService = new Mock<ICacheService>();
            _eventService = new EventService(_mockEventRepository.Object, _mockObjectStorageService.Object, _mockCacheService.Object);
        }

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnEvents()
        {
            // Arrange
            var expectedEvents = new List<Event>
            {
                new Event { Id = 1, Name = "Event 1" },
                new Event { Id = 2, Name = "Event 2" }
            };
            _mockEventRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(expectedEvents);

            // Act
            var result = await _eventService.GetAllEventsAsync();

            // Assert
            Assert.Equal(expectedEvents.Count, result.Count());
        }

        [Fact]
        public async Task GetEventById_ShouldReturnEvent_WhenExists()
        {
            // Arrange
            var eventId = 1;
            var expectedEvent = new Event { Id = eventId, Name = "Test Event" };
            _mockEventRepository.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(expectedEvent);

            // Act
            var result = await _eventService.GetEventById(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.Id);
        }

        [Fact]
        public async Task CreateEvent_ShouldCreateEventAndSeats()
        {
            // Arrange
            var request = new AddUpdateEventRequest
            {
                Name = "New Event",
                Description = "Test Description",
                SeatPrice = 100
            };

            var createdEvent = new Event { Id = 1, Name = request.Name };
            _mockEventRepository.Setup(x => x.AddAsync(It.IsAny<Event>())).ReturnsAsync(createdEvent);
            _mockEventRepository.Setup(x => x.RegisterSeatsForEventAsync(It.IsAny<CreateSeatDto>()))
                .ReturnsAsync(("success", 160));

            // Act
            var result = await _eventService.CreateEvent(request);

            // Assert
            Assert.Equal(createdEvent.Id, result);
            _mockEventRepository.Verify(x => x.SaveChangeAsync(), Times.Once);
            _mockEventRepository.Verify(x => x.RegisterSeatsForEventAsync(It.Is<CreateSeatDto>(dto =>
                dto.EventId == createdEvent.Id &&
                dto.Price == request.SeatPrice)), Times.Once);
        }

        [Fact]
        public async Task UpdateEvent_ShouldReturnTrue_WhenEventExists()
        {
            // Arrange
            var request = new AddUpdateEventRequest
            {
                ID = 1,
                Name = "Updated Event"
            };

            var existingEvent = new Event { Id = request.ID, Name = "Original Event" };
            _mockEventRepository.Setup(x => x.GetByIdAsync(request.ID)).ReturnsAsync(existingEvent);
            _mockEventRepository.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            // Act
            var result = await _eventService.UpdateEvent(request);

            // Assert
            Assert.True(result);
            _mockEventRepository.Verify(x => x.Update(It.Is<Event>(e =>
                e.Id == request.ID &&
                e.Name == request.Name)), Times.Once);
        }

        [Fact]
        public async Task DeleteEvent_ShouldReturnTrue_WhenEventExists()
        {
            // Arrange
            var eventId = 1;
            var eventToDelete = new Event { Id = eventId };
            _mockEventRepository.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(eventToDelete);
            _mockEventRepository.Setup(x => x.SaveChangeAsync()).ReturnsAsync(1);

            // Act
            var result = await _eventService.DeleteEvent(eventId);

            // Assert
            Assert.True(result);
            _mockEventRepository.Verify(x => x.Delete(eventToDelete), Times.Once);
        }

        [Fact]
        public async Task GetFilteredPagedEventsAsync_ShouldReturnUpcomingEvents_WhenIsUpcomingTrue()
        {
            // Arrange
            var request = new EventSearchParamsRequest { IsUpcoming = true };
            var upcomingEvents = new List<Event>
            {
                new Event { Id = 1, Name = "Upcoming Event" }
            };

            _mockCacheService.Setup(x => x.GetAsync<List<Event>>(CacheKeyConsts.UPCOMING_EVENTS))
                .ReturnsAsync((List<Event>?)null);
            _mockEventRepository.Setup(x => x.GetUpcomingEventsAsync())
                .ReturnsAsync(upcomingEvents);

            // Act
            var result = await _eventService.GetFilteredPagedEventsAsync(request);

            // Assert
            Assert.Single(result);
            _mockCacheService.Verify(x => x.SetAsync(
                CacheKeyConsts.UPCOMING_EVENTS,
                upcomingEvents,
                It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public async Task GetEventInfoWithSeatAsync_ShouldReturnEventBookingInfo()
        {
            // Arrange
            var eventId = 1;
            var expectedInfo = new EventBookingInfoDto
            {
                EventInfo = new EventInfoDto { EventID = eventId },
                SeatInfos = new List<SeatInfoDto>()
            };

            _mockEventRepository.Setup(x => x.GetEventInfoWithSeatsByEventIDAsync(eventId))
                .ReturnsAsync(expectedInfo);

            // Act
            var result = await _eventService.GetEventInfoWithSeatAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(eventId, result.EventInfo.EventID);
        }

        [Fact]
        public async Task RegisterSeats_ShouldReturnMessageAndTotalSeats()
        {
            // Arrange
            var createSeatDto = new CreateSeatDto
            {
                EventId = 1,
                Price = 100
            };
            var expectedMessage = "registed successfully.";
            var expectedTotalSeats = 160;

            _mockEventRepository.Setup(x => x.RegisterSeatsForEventAsync(createSeatDto))
                .ReturnsAsync((expectedMessage, expectedTotalSeats));

            // Act
            var (message, totalSeats) = await _eventService.RegisterSeats(createSeatDto);

            // Assert
            Assert.Equal(expectedMessage, message);
            Assert.Equal(expectedTotalSeats, totalSeats);
        }

        [Fact]
        public async Task GetAllEventAsync_ShouldReturnAllEvents()
        {
            // Arrange
            var expectedEvents = new List<Event>
    {
        new Event { Id = 1, Name = "Event 1" },
        new Event { Id = 2, Name = "Event 2" }
    };
            _mockEventRepository.Setup(x => x.GetAllAsync())
                .ReturnsAsync(expectedEvents);

            // Act
            var result = await _eventService.GetAllEventAsync();

            // Assert
            var events = result.ToList();
            Assert.Equal(expectedEvents.Count, events.Count);
            Assert.Equal(expectedEvents[0].Id, events[0].Id);
            Assert.Equal(expectedEvents[0].Name, events[0].Name);
            Assert.Equal(expectedEvents[1].Id, events[1].Id);
            Assert.Equal(expectedEvents[1].Name, events[1].Name);
            _mockEventRepository.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetEventDetailByIdAsync_ShouldReturnEvent_WhenEventExists()
        {
            // Arrange
            var eventId = 1;
            var expectedEvent = new Event
            {
                Id = eventId,
                Name = "Test Event",
                Description = "Test Description",
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                VenueName = "Test Venue",
                VenueAddress = "Test Address"
            };

            _mockEventRepository.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync(expectedEvent);

            // Act
            var result = await _eventService.GetEventDetailByIdAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEvent.Id, result.Id);
            Assert.Equal(expectedEvent.Name, result.Name);
            Assert.Equal(expectedEvent.Description, result.Description);
            Assert.Equal(expectedEvent.VenueName, result.VenueName);
            Assert.Equal(expectedEvent.VenueAddress, result.VenueAddress);
            _mockEventRepository.Verify(x => x.GetByIdAsync(eventId), Times.Once);
        }

        [Fact]
        public async Task GetEventDetailByIdAsync_ShouldReturnNull_WhenEventDoesNotExist()
        {
            // Arrange
            var eventId = 999;
            _mockEventRepository.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync((Event?)null);

            // Act
            var result = await _eventService.GetEventDetailByIdAsync(eventId);

            // Assert
            Assert.Null(result);
            _mockEventRepository.Verify(x => x.GetByIdAsync(eventId), Times.Once);
        }
    }
}