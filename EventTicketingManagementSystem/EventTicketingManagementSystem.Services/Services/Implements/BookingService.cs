using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;

namespace EventTicketingManagementSystem.Services.Services.Implements
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository bookingRepository;
        private readonly IPaymentRepository paymentRepository;
        private readonly ITicketRepository ticketRepository;
        private readonly ISeatRepository seatRepository;

        public BookingService(IBookingRepository bookingRepository, IPaymentRepository paymentRepository, ITicketRepository ticketRepository, ISeatRepository seatRepository)
        {
            this.bookingRepository = bookingRepository;
            this.paymentRepository = paymentRepository;
            this.ticketRepository = ticketRepository;
            this.seatRepository = seatRepository;
        }

        public async Task RemovePendingExpiredBookings()
        {
            var expiredBookings = await bookingRepository.GetPendingExpiredBookingsAsync();
            foreach (var booking in expiredBookings)
            {
                bookingRepository.Delete(booking);
                paymentRepository.DeleteRange(booking.Payments);
                ticketRepository.DeleteRange(booking.Tickets);

                foreach (var seat in booking.Seats)
                {
                    seat.Status = CommConstants.CST_SEAT_STATUS_DEFAULT;
                }
                seatRepository.UpdateRange(booking.Seats);
            }

            await bookingRepository.SaveChangeAsync();
        }
    }
}
