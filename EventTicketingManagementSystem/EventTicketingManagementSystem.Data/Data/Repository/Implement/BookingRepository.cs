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
                    //Status = "pending for payment",
                    Status = CommConstants.CST_PAY_STATUS_PAID
                };

                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();

                //var payment = new Payment
                //{
                //    BookingId = newBooking.Id,
                //    Amount = newBooking.TotalAmount,
                //    //PaymentMethod = "Not Selected",
                //    //Status = "Pending",
                //    PaymentMethod = "Bank",
                //    Status = "Done",
                //    TransactionId = null,
                //    PaymentDate = null,
                //    RefundDate = null
                //};

                //_context.Payments.Add(payment);
                //await _context.SaveChangesAsync();

                foreach (var seatInfo in bookingRequestDto.SeatedInfos)
                {
                    var seat = await _context.Seats
                        .FirstOrDefaultAsync(s => s.Id == seatInfo.SeatId && s.EventId == seatInfo.EventId);

                    if (seat != null)
                    {
                        seat.Status = CommConstants.CST_SEAT_STATUS_BOOKED;
                        _context.Seats.Update(seat);
                    }
                }

                await _context.SaveChangesAsync();

                var tickets = bookingRequestDto.SeatedInfos.Select(seat => new Ticket
                {
                    BookingId = newBooking.Id,
                    SeatId = seat.SeatId,
                    TicketNumber = GenerateTicketNumber(seat.Row, seat.Number),
                    Status = CommConstants.CST_SEAT_STATUS_RESERVED,
                    ReservedAt = DateTime.UtcNow
                }).ToList();

                await _context.Tickets.AddRangeAsync(tickets);
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

        private string GenerateTicketNumber(string row, int number)
        {
            string dateNow = DateTime.UtcNow.ToString("yyyyMMdd");
            string randomCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            return $"TICKET-{randomCode}-{dateNow}-{row}{number:D2}";
        }

    }
}
