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
                if (booking.Payments != null && booking.Payments.Count > 0)
                    paymentRepository.DeleteRange(booking.Payments);

                if (booking.Tickets != null && booking.Tickets.Count > 0)
                    ticketRepository.DeleteRange(booking.Tickets);

                if (booking.Seats != null && booking.Seats.Count > 0)
                {
                    foreach (var seat in booking.Seats)
                    {
                        seat.BookingId = null;
                        seat.Status = CommConstants.CST_SEAT_STATUS_DEFAULT;
                    }
                    seatRepository.UpdateRange(booking.Seats);
                }

                bookingRepository.Delete(booking);
            }

            await bookingRepository.SaveChangeAsync();
        }
    }
}
