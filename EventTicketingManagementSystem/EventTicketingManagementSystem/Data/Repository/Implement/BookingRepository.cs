using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Repository.Implement
{
    public class BookingRepository : GenericRepository<Booking, int>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<BookingInfoDto>> GetBookingInfosByUserIdAsync(int userId)
        {
            var bookingInfos = await _context.Bookings
                .AsNoTracking()
                .Include(b => b.Tickets)
                .ThenInclude(t => t.Seat)
                .Where(b => b.UserId == userId)
                .Select(b => new BookingInfoDto
                {
                    BookingId = b.Id,
                    EventId = b.EventId,
                    EventName = b.Event.Name,
                    EventDate = b.Event.StartDate.Date.ToString(),
                    EventTime = b.Event.StartDate.TimeOfDay.ToString(),
                    TotalAmount = b.TotalAmount,
                    Status = b.Status,
                    BookedAt = b.CreatedAt,
                    Tickets = b.Tickets.Select(t => new TicketInfoDto
                    {
                        TicketId = t.Id,
                        TicketNumber = t.TicketNumber,
                        Status = t.Status,
                        SeatType = t.Seat.Type,
                        TicketPrice = t.Seat.Price,
                    }).ToList()
                })
                .ToListAsync();

            return bookingInfos ?? new List<BookingInfoDto>();
        }
    }
}
