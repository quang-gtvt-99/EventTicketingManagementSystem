using System.Diagnostics.CodeAnalysis;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Data.Repository.Implement
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
                    Venue = b.Event.VenueAddress,
                    EventName = b.Event.Name,
                    EventDate = $"{b.Event.StartDate.Date.ToString("dd/MM/yyyy")} - {b.Event.EndDate.Date.ToString("dd/MM/yyyy")}",
                    EventTime = b.Event.StartDate.TimeOfDay.ToString(),
                    TotalAmount = b.Subtotal,
                    Status = b.Status,
                    BookedAt = b.CreatedAt,
                    Tickets = b.Tickets.Select(t => new TicketInfoDto
                    {
                        TicketId = t.Id,
                        Row = t.Seat.Row,
                        Number = t.Seat.Number,
                        TicketNumber = t.TicketNumber,
                        Status = t.Status,
                        SeatType = t.Seat.Type,
                        TicketPrice = t.Seat.Price,
                    }).ToList()
                })
                .ToListAsync();

            return bookingInfos ?? new List<BookingInfoDto>();
        }

        [ExcludeFromCodeCoverage]
        public async Task<Booking> CreateBookingAsync(CreateBookingDto bookingRequestDto, int loggedInUserId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userExists = await _context.Users.AnyAsync(u => u.Id == loggedInUserId);
                if (!userExists)
                {
                    throw new Exception("User does not exist.");
                }

                var newBooking = new Booking
                {
                    UserId = loggedInUserId,
                    EventId = bookingRequestDto.EventId,
                    Subtotal = bookingRequestDto.SeatedInfos.Sum(s => s.Price),
                    Quantity = bookingRequestDto.SeatedInfos.Count,
                    TotalAmount = bookingRequestDto.SeatedInfos.Sum(s => s.Price) * 1.05m, //include tax 5%
                    BookingDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMinutes(15), //expriy after 15 minutes
                    Status = CommConstants.CST_PAY_STATUS_PAID
                };

                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();

                foreach (var seatInfo in bookingRequestDto.SeatedInfos)
                {
                    var seat = await _context.Seats
                        .FirstOrDefaultAsync(s => s.Id == seatInfo.SeatId && s.EventId == seatInfo.EventId);

                    if (seat != null)
                    {
                        seat.Status = CommConstants.CST_SEAT_STATUS_BOOKED;
                        seat.BookingId = newBooking.Id;
                        _context.Seats.Update(seat);
                    }
                }

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return newBooking;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }
        public async Task<IEnumerable<Booking>> GetPendingExpiredBookingsAsync()
        {
            return await _context.Bookings
                .Include(x => x.Payments)
                .Include(x => x.Tickets)
                .Include(x => x.Seats)
                .Where(b => b.Status == CommConstants.CST_PAY_STATUS_PENDING 
                    && b.ExpiryDate < DateTime.UtcNow)
                .ToListAsync();
        }
        public async Task<bool> DeleteBookingByIdAsync(int bookingId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Tickets)
                .Include(b => b.Seats)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return false;
            }

            // Remove related entities first
            if (booking.Tickets != null)
            {
                _context.Tickets.RemoveRange(booking.Tickets);
            }

            if (booking.Seats != null)
            {
                foreach (var seat in booking.Seats)
                {
                    seat.BookingId = null;
                    seat.Status = CommConstants.CST_SEAT_STATUS_DEFAULT;
                }
            }

            if (booking.Payments != null)
            {
                _context.Payments.RemoveRange(booking.Payments);
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
