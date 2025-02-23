using EventTicketingManagementSystem.Data.Data.Repository.Implement;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystemTests.Data.Repository
{
    public class SeatRepositoryTests : BaseRepositoryTests
    {
        private readonly SeatRepository _seatRepository;

        public SeatRepositoryTests()
        {
            _seatRepository = new SeatRepository(Context);
            SeedData();
        }

        private void SeedData()
        {
            var eventEntity = new Event
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
                    Type = CommConstants.CST_SEAT_TYPE_NOR
                },
                new Seat
                {
                    Id = 2,
                    EventId = 1,
                    Row = "A", 
                    Number = 2,
                    Status = CommConstants.CST_SEAT_STATUS_BOOKED,
                    BookingId = 1,
                    Type = CommConstants.CST_SEAT_TYPE_VIP
                }
            };

            Context.Events.Add(eventEntity);
            Context.Bookings.Add(booking);
            Context.Seats.AddRange(seats);
            Context.SaveChanges();
        }

        [Fact]
        public async Task UpdateSeatsByBookingIdAsync_ShouldUpdateSeatsStatus_WhenBookingExists()
        {
            // Arrange
            var bookingId = 1;
            var newStatus = CommConstants.CST_SEAT_STATUS_DEFAULT;

            // Act
            var updatedSeats = await _seatRepository.UpdateSeatsByBookingIdAsync(bookingId, newStatus);

            // Assert
            Assert.NotNull(updatedSeats);
            Assert.Equal(2, updatedSeats.Count);
            Assert.All(updatedSeats, seat =>
            {
                Assert.Null(seat.BookingId);
                Assert.Equal(newStatus, seat.Status);
            });

            // Verify changes persisted to database
            var seatsInDb = await Context.Seats.Where(s => s.Id == 1 || s.Id == 2).ToListAsync();
            Assert.All(seatsInDb, seat =>
            {
                Assert.Null(seat.BookingId);
                Assert.Equal(newStatus, seat.Status);
            });
        }

        [Fact]
        public async Task UpdateSeatsByBookingIdAsync_ShouldReturnEmptyList_WhenBookingDoesNotExist()
        {
            // Arrange
            var nonExistentBookingId = 999;
            var newStatus = CommConstants.CST_SEAT_STATUS_DEFAULT;

            // Act
            var updatedSeats = await _seatRepository.UpdateSeatsByBookingIdAsync(nonExistentBookingId, newStatus);

            // Assert
            Assert.NotNull(updatedSeats);
            Assert.Empty(updatedSeats);
        }
    }
}