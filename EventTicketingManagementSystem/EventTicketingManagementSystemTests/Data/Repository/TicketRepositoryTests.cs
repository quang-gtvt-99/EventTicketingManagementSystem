using EventTicketingManagementSystem.Data.Data.Repository.Implement;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystemTests.Data.Repository
{
    public class TicketRepositoryTests : BaseRepositoryTests
    {
        private readonly TicketRepository _ticketRepository;

        public TicketRepositoryTests()
        {
            _ticketRepository = new TicketRepository(Context);
            SeedData();
        }

        private void SeedData()
        {
            var @event = new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "Test Description",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2)
            };

            var booking = new Booking
            {
                Id = 1,
                EventId = 1,
                UserId = 1,
                BookingDate = DateTime.UtcNow,
                Status = CommConstants.CST_PAY_STATUS_PENDING
            };

            var seats = new List<Seat>
            {
                new Seat
                {
                    Id = 1,
                    EventId = 1,
                    Row = "A",
                    Number = 1,
                    Status = CommConstants.CST_SEAT_STATUS_BOOKED,
                    BookingId = 1,
                    Type = CommConstants.CST_SEAT_TYPE_NOR  // Add Type property
                },
                new Seat
                {
                    Id = 2,
                    EventId = 1,
                    Row = "A",
                    Number = 2,
                    Status = CommConstants.CST_SEAT_STATUS_BOOKED,
                    BookingId = 1,
                    Type = CommConstants.CST_SEAT_TYPE_VIP  // Add Type property
                }
            };

            Context.Events.Add(@event);
            Context.Bookings.Add(booking);
            Context.Seats.AddRange(seats);
            Context.SaveChanges();
        }

        [Fact]
        public async Task CreateTicketsAsync_ShouldCreateTickets_WhenValidBookingAndSeatsExist()
        {
            // Arrange
            var bookingId = 1;

            // Act
            var tickets = await _ticketRepository.CreateTicketsAsync(bookingId);

            // Assert
            Assert.NotNull(tickets);
            Assert.Equal(2, tickets.Count);
            Assert.All(tickets, ticket =>
            {
                Assert.Equal(bookingId, ticket.BookingId);
                Assert.Equal(CommConstants.CST_SEAT_STATUS_RESERVED, ticket.Status);
                Assert.NotNull(ticket.TicketNumber);
                Assert.StartsWith("TICKET-", ticket.TicketNumber);
            });
        }

        [Fact]
        public async Task CreateTicketsAsync_ShouldThrowKeyNotFoundException_WhenBookingDoesNotExist()
        {
            // Arrange
            var nonExistentBookingId = 999;

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                async () => await _ticketRepository.CreateTicketsAsync(nonExistentBookingId)
            );
        }

        [Fact]
        public async Task CreateTicketsAsync_ShouldThrowInvalidOperationException_WhenNoAvailableSeats()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 2,
                EventId = 1,
                UserId = 1,
                BookingDate = DateTime.UtcNow,
                Status = CommConstants.CST_PAY_STATUS_PENDING
            };
            Context.Bookings.Add(booking);
            await Context.SaveChangesAsync();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _ticketRepository.CreateTicketsAsync(booking.Id)
            );
        }
    }
}