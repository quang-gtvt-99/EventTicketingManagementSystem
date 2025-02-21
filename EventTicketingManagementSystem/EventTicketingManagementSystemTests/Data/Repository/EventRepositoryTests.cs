using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.Data.Data.Repository.Implement;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Models;
using EventTicketingMananagementSystem.Core.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystemTests.Data.Repository
{
    public class EventRepositoryTests : BaseRepositoryTests
    {
        private readonly EventRepository _eventRepository;

        public EventRepositoryTests()
        {
            _eventRepository = new EventRepository(Context);
            SeedData();
        }

        private void SeedData()
        {
            var events = new List<Event>
            {
                new Event
                {
                    Id = 1,
                    Name = "Concert A",
                    Description = "Rock concert",
                    Category = "Music",
                    Status = "Upcoming",
                    StartDate = DateTime.UtcNow.AddDays(1),
                    EndDate = DateTime.UtcNow.AddDays(2),
                    VenueName = "Stadium 1",
                    VenueAddress = "Address 1"
                },
                new Event
                {
                    Id = 2,
                    Name = "Theatre Show",
                    Description = "Drama show",
                    Category = "Theatre",
                    Status = "Completed",
                    StartDate = DateTime.UtcNow.AddDays(-2),
                    EndDate = DateTime.UtcNow.AddDays(-1),
                    VenueName = "Theatre Hall",
                    VenueAddress = "Address 2"
                }
            };

            Context.Events.AddRange(events);
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetEventsByFilter_ShouldReturnFilteredEvents_WhenFiltersApplied()
        {
            // Arrange
            var search = "Concert";
            var category = "Music";
            var status = "Upcoming";

            // Act
            var result = await _eventRepository.GetEventsByFilter(search, category, status);

            // Assert
            Assert.NotNull(result);
            var events = result.ToList();
            Assert.Single(events);
            Assert.All(events, e =>
            {
                Assert.Contains(search, e.Name, StringComparison.OrdinalIgnoreCase);
                Assert.Equal(category, e.Category);
                Assert.Equal(status, e.Status);
            });
        }

        [Fact]
        public async Task GetFilteredPagedAsync_ShouldReturnFilteredEvents_WhenSearchParamsProvided()
        {
            // Arrange
            var searchParams = new EventSearchParamsRequest
            {
                Search = "show",
                Category = "Theatre",
                Status = "Completed"
            };

            // Act
            var result = await _eventRepository.GetFilteredPagedAsync(searchParams);

            // Assert
            Assert.NotNull(result);
            var events = result.ToList();
            Assert.Single(events);
            Assert.All(events, e =>
            {
                Assert.Contains(searchParams.Search, e.Name, StringComparison.OrdinalIgnoreCase);
                Assert.Equal(searchParams.Category, e.Category);
                Assert.Equal(searchParams.Status, e.Status);
            });
        }

        [Fact]
        public async Task CountSearch_ShouldReturnCorrectCount_WhenSearchTermProvided()
        {
            // Arrange
            var search = "show";

            // Act
            var count = await _eventRepository.CountSearch(search);

            // Assert
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task GetEventInfoWithSeatsByEventIDAsync_ShouldReturnEventInfo_WhenEventExists()
        {
            // Arrange
            var eventId = 1;
            var seats = new List<Seat>
            {
                new Seat { Id = 1, EventId = 1, Row = "A", Number = 1, Type = "VIP", Price = 100, Status = "Available" },
                new Seat { Id = 2, EventId = 1, Row = "A", Number = 2, Type = "Normal", Price = 50, Status = "Available" }
            };
            Context.Seats.AddRange(seats);
            Context.SaveChanges();

            // Act
            var result = await _eventRepository.GetEventInfoWithSeatsByEventIDAsync(eventId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.EventInfo);
            Assert.Equal("Concert A", result.EventInfo.EventName);
            Assert.Equal(2, result.SeatInfos.Count);
        }

        [Fact]
        public async Task RegisterSeatsForEventAsync_ShouldCreateSeats_WhenValidRequestProvided()
        {
            // Arrange
            var createSeatDto = new CreateSeatDto
            {
                EventId = 1,
                Price = 100
            };

            // Act
            var (message, totalSeats) = await _eventRepository.RegisterSeatsForEventAsync(createSeatDto);

            // Assert
            Assert.Equal("registed successfully.", message);
            Assert.True(totalSeats > 0);
            var seatsInDb = await Context.Seats.Where(s => s.EventId == createSeatDto.EventId).ToListAsync();
            Assert.Equal(totalSeats, seatsInDb.Count);
        }

        [Fact]
        public async Task GetUpcomingEventsAsync_ShouldReturnUpcomingEvents()
        {
            // Act
            var result = await _eventRepository.GetUpcomingEventsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.All(result, e => Assert.True(e.StartDate > DateTime.UtcNow));
        }
    }
}