using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Data.Data.Repository.Implement
{
    public class PaymentRepository : GenericRepository<Payment, int>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<Payment> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentDto requestDto)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                throw new KeyNotFoundException("Payment not found.");
            }
            var booking = await _context.Bookings.FindAsync(payment.BookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            if (booking.ExpiryDate < DateTime.UtcNow)
            {
                throw new InvalidOperationException("expired date.");
            }

            payment.Status = requestDto.Status;
            payment.TransactionId = requestDto.TransactionId;
            payment.PaymentMethod = requestDto.PaymentMethod;
            payment.PaymentDate = DateTime.UtcNow;
            payment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return payment;
        }
        public async Task<bool> DeleteExpiredBookingAsync(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null)
            {
                throw new KeyNotFoundException("Payment not found.");
            }

            var booking = await _context.Bookings.FindAsync(payment.BookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            if (booking.ExpiryDate < DateTime.UtcNow)
            {
                _context.Payments.Remove(payment);
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
