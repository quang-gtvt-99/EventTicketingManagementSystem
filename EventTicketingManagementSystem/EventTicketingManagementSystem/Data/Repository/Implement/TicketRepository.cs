using EventTicketingManagementSystem.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Repository.Implement
{
    public class TicketRepository : GenericRepository<Ticket, int>, ITicketRepository
    {
        public TicketRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Ticket>> CreateTicketsAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            var seats = await _context.Seats
            .Include(s => s.Event)
            .Where(s => s.EventId == booking.EventId
                      && s.Status == "available"
                      && s.Event.Bookings.Select(b => b.Id).Contains(booking.Id))
            .ToListAsync();

            if (!seats.Any())
            {
                throw new InvalidOperationException("No available seats.");
            }

            var tickets = seats.Select(seat => new Ticket
            {
                BookingId = bookingId,
                SeatId = seat.Id,
                TicketNumber = GenerateTicketNumber(),
                Status = "reserved",
                ReservedAt = DateTime.UtcNow
            }).ToList();

            await _context.Tickets.AddRangeAsync(tickets);
            await _context.SaveChangesAsync();

            return tickets;
        }
        private string GenerateTicketNumber()
        {
            return $"TCK-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }
    }
}
