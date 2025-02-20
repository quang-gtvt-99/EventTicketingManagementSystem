using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Services.Services.Implements;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Models;
using Moq;
using Xunit;

namespace EventTicketingManagementSystemTests.Service
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<IPaymentRepository> _mockPaymentRepository;
        private readonly Mock<ITicketRepository> _mockTicketRepository;
        private readonly Mock<ISeatRepository> _mockSeatRepository;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockPaymentRepository = new Mock<IPaymentRepository>();
            _mockTicketRepository = new Mock<ITicketRepository>();
            _mockSeatRepository = new Mock<ISeatRepository>();

            _bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockPaymentRepository.Object,
                _mockTicketRepository.Object,
                _mockSeatRepository.Object
            );
        }

        [Fact]
        public async Task RemovePendingExpiredBookings_ShouldDeleteBookingsAndRelatedData()
        {
            // Arrange
            var expiredBookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    Payments = new List<Payment> { new Payment { Id = 1 } },
                    Tickets = new List<Ticket> { new Ticket { Id = 1 } },
                    Seats = new List<Seat>
                    {
                        new Seat { Id = 1, Status = CommConstants.CST_SEAT_STATUS_BOOKED }
                    }
                }
            };

            _mockBookingRepository.Setup(x => x.GetPendingExpiredBookingsAsync())
                .ReturnsAsync(expiredBookings);

            // Act
            await _bookingService.RemovePendingExpiredBookings();

            // Assert
            _mockPaymentRepository.Verify(x => x.DeleteRange(
                It.Is<List<Payment>>(p => p.Count == 1)), Times.Once);

            _mockTicketRepository.Verify(x => x.DeleteRange(
                It.Is<List<Ticket>>(t => t.Count == 1)), Times.Once);

            _mockSeatRepository.Verify(x => x.UpdateRange(
                It.Is<List<Seat>>(s => 
                    s.Count == 1 && 
                    s[0].Status == CommConstants.CST_SEAT_STATUS_DEFAULT && 
                    s[0].BookingId == null)), 
                Times.Once);

            _mockBookingRepository.Verify(x => x.Delete(
                It.Is<Booking>(b => b.Id == 1)), Times.Once);

            _mockBookingRepository.Verify(x => x.SaveChangeAsync(), Times.Once);
        }

        [Fact]
        public async Task RemovePendingExpiredBookings_ShouldHandleEmptyBookings()
        {
            // Arrange
            _mockBookingRepository.Setup(x => x.GetPendingExpiredBookingsAsync())
                .ReturnsAsync(new List<Booking>());

            // Act
            await _bookingService.RemovePendingExpiredBookings();

            // Assert
            _mockPaymentRepository.Verify(x => x.DeleteRange(It.IsAny<List<Payment>>()), Times.Never);
            _mockTicketRepository.Verify(x => x.DeleteRange(It.IsAny<List<Ticket>>()), Times.Never);
            _mockSeatRepository.Verify(x => x.UpdateRange(It.IsAny<List<Seat>>()), Times.Never);
            _mockBookingRepository.Verify(x => x.Delete(It.IsAny<Booking>()), Times.Never);
            _mockBookingRepository.Verify(x => x.SaveChangeAsync(), Times.Once);
        }

        [Fact]
        public async Task RemovePendingExpiredBookings_ShouldHandleBookingsWithoutRelatedData()
        {
            // Arrange
            var expiredBookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    Payments = new List<Payment>(),
                    Tickets = new List<Ticket>(),
                    Seats = new List<Seat>()
                }
            };

            _mockBookingRepository.Setup(x => x.GetPendingExpiredBookingsAsync())
                .ReturnsAsync(expiredBookings);

            // Act
            await _bookingService.RemovePendingExpiredBookings();

            // Assert
            _mockPaymentRepository.Verify(x => x.DeleteRange(It.IsAny<List<Payment>>()), Times.Never);
            _mockTicketRepository.Verify(x => x.DeleteRange(It.IsAny<List<Ticket>>()), Times.Never);
            _mockSeatRepository.Verify(x => x.UpdateRange(It.IsAny<List<Seat>>()), Times.Never);
            _mockBookingRepository.Verify(x => x.Delete(It.Is<Booking>(b => b.Id == 1)), Times.Once);
            _mockBookingRepository.Verify(x => x.SaveChangeAsync(), Times.Once);
        }
    }
}