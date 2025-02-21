using EventTicketingManagementSystem.Data.Data.Repository.Implement;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Models;
using EventTicketingMananagementSystem.Core.Dtos;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystemTests.Data.Repository
{
    public class BookingRepositoryTests : BaseRepositoryTests
    {
        private readonly BookingRepository _bookingRepository;

        public BookingRepositoryTests()
        {
            _bookingRepository = new BookingRepository(Context);
            SeedData();
        }

        private void SeedData()
        {
            var user = new User
            {
                Id = 1,
                Email = "test@example.com",
                FullName = "Test User",
                PasswordHash = "hashedpassword123", // Required property
                PhoneNumber = "1234567890",        // Required property
                Status = "Active"
            };

            var event1 = new Event
            {
                Id = 1,
                Name = "Test Event",
                Description = "Test Description",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                VenueAddress = "Test Venue",
                Status = "Upcoming",
                CreatedAt = DateTime.UtcNow
            };

            var booking = new Booking
            {
                Id = 1,
                UserId = 1,
                EventId = 1,
                BookingDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(15),
                Subtotal = 100000,
                TotalAmount = 105000,
                Quantity = 2,
                CreatedAt = DateTime.UtcNow
            };

            var tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    BookingId = 1,
                    SeatId = 1,
                    TicketNumber = "TICKET-001",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = 2,
                    BookingId = 1,
                    SeatId = 2,
                    TicketNumber = "TICKET-002",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow
                }
            };

            var seats = new List<Seat>
            {
                new Seat
                {
                    Id = 1,
                    EventId = 1,
                    Row = "A",
                    Number = 1,
                    Type = CommConstants.CST_SEAT_TYPE_NOR,
                    Price = 50000,
                    Status = CommConstants.CST_SEAT_STATUS_BOOKED,
                    BookingId = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Seat
                {
                    Id = 2,
                    EventId = 1,
                    Row = "A",
                    Number = 2,
                    Type = CommConstants.CST_SEAT_TYPE_VIP,
                    Price = 50000,
                    Status = CommConstants.CST_SEAT_STATUS_BOOKED,
                    BookingId = 1,
                    CreatedAt = DateTime.UtcNow
                }
            };

            Context.Users.Add(user);
            Context.Events.Add(event1);
            Context.Bookings.Add(booking);
            Context.Tickets.AddRange(tickets);
            Context.Seats.AddRange(seats);
            Context.SaveChanges();
        }

        [Fact]
        public async Task GetBookingInfosByUserIdAsync_ShouldReturnBookingInfos_WhenBookingsExist()
        {
            // Arrange
            var userId = 1;

            // Act
            var bookingInfos = await _bookingRepository.GetBookingInfosByUserIdAsync(userId);

            // Assert
            Assert.NotNull(bookingInfos);
            Assert.Single(bookingInfos);
            var bookingInfo = bookingInfos.First();
            Assert.Equal(1, bookingInfo.BookingId);
            Assert.Equal(2, bookingInfo.Tickets.Count);
            Assert.Equal(100000, bookingInfo.TotalAmount);
        }

        [Fact]
        public async Task GetBookingInfosByUserIdAsync_ShouldReturnEmptyList_WhenNoBookingsExist()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act
            var bookingInfos = await _bookingRepository.GetBookingInfosByUserIdAsync(nonExistentUserId);

            // Assert
            Assert.NotNull(bookingInfos);
            Assert.Empty(bookingInfos);
        }

        [Fact]
        public async Task GetPendingExpiredBookingsAsync_ShouldReturnExpiredBookings()
        {
            // Arrange
            var expiredBooking = new Booking
            {
                Id = 2,
                UserId = 1,
                EventId = 1,
                BookingDate = DateTime.UtcNow.AddHours(-1),
                ExpiryDate = DateTime.UtcNow.AddMinutes(-30)
            };
            Context.Bookings.Add(expiredBooking);
            await Context.SaveChangesAsync();

            // Act
            var result = await _bookingRepository.GetPendingExpiredBookingsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            var booking = result.First();
            Assert.True(booking.ExpiryDate < DateTime.UtcNow);
        }

        [Fact]
        public async Task DeleteBookingByIdAsync_ShouldReturnTrue_WhenBookingExists()
        {
            // Arrange
            var bookingId = 1;

            // Act
            var result = await _bookingRepository.DeleteBookingByIdAsync(bookingId);

            // Assert
            Assert.True(result);
            Assert.Null(await Context.Bookings.FindAsync(bookingId));

            // Verify related entities
            Assert.Empty(await Context.Tickets.Where(t => t.BookingId == bookingId).ToListAsync());
            Assert.Empty(await Context.Payments.Where(p => p.BookingId == bookingId).ToListAsync());

            // Verify seats are updated but not deleted
            var seats = await Context.Seats.Where(s => s.EventId == 1).ToListAsync();
            Assert.All(seats, seat =>
            {
                Assert.Null(seat.BookingId);
                Assert.Equal(CommConstants.CST_SEAT_STATUS_DEFAULT, seat.Status);
            });
        }

        [Fact]
        public async Task DeleteBookingByIdAsync_ShouldReturnFalse_WhenBookingDoesNotExist()
        {
            // Arrange
            var nonExistentBookingId = 999;

            // Act
            var result = await _bookingRepository.DeleteBookingByIdAsync(nonExistentBookingId);

            // Assert
            Assert.False(result);
        }
    }
}