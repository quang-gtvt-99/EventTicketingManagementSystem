using EventTicketingManagementSystem.Data.Repository.Interfaces;
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
        public async Task<Booking> CreateBookingAsync(CreateBookingDto bookingRequestDto, int loggedInUserId)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == loggedInUserId);
            if (!userExists)
            {
                throw new Exception("User does not exist.");
            }

            if (bookingRequestDto.UserId != loggedInUserId)
            {
                throw new UnauthorizedAccessException("Booking fail.");
            }

            var newBooking = new Booking
            {
                UserId = bookingRequestDto.UserId,
                EventId = bookingRequestDto.EventId,
                Subtotal = bookingRequestDto.SeatedInfos.Sum(s => s.Price),
                Quantity = bookingRequestDto.SeatedInfos.Count,
                TotalAmount = bookingRequestDto.SeatedInfos.Sum(s => s.Price) * 0.3m, //include tax 3%
                BookingDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMinutes(30), //expriy after 30 minutes
                Status = "pending for payment",
                UnitPrice = 0 // Unused
            };

            _context.Bookings.Add(newBooking);
            await _context.SaveChangesAsync();

            var payment = new Payment
            {
                BookingId = newBooking.Id,
                Amount = newBooking.TotalAmount,
                PaymentMethod = "Not Selected",
                Status = "Pending",
                TransactionId = null,
                PaymentDate = null,
                RefundDate = null
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return newBooking;
        }
    }
}
