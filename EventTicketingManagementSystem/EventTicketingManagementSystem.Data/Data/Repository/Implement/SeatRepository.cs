using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Data.Repository.Implement
{
    public class SeatRepository : GenericRepository<Seat, int>, ISeatRepository
    {
        public SeatRepository(AppDbContext context) : base(context)
        {

        }
        public async Task<List<Seat>> UpdateSeatsByBookingIdAsync(int bookingId, string status)
        {
            var seats = await _context.Seats.Where(s => s.BookingId == bookingId).ToListAsync();

            foreach (var seat in seats)
            {
                seat.BookingId = null;
                seat.Status = status;
            }

            await _context.SaveChangesAsync();

            return seats;
        }
    }
}
