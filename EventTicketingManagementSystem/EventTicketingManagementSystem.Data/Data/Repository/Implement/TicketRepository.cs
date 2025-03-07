﻿using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Data.Repository.Implement
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
                      && s.Status == CommConstants.CST_SEAT_STATUS_BOOKED
                      && s.BookingId == bookingId)
            .ToListAsync();

            if (!seats.Any())
            {
                throw new InvalidOperationException("No available seats.");
            }

            var tickets = seats.Select(seat => new Ticket
            {
                BookingId = bookingId,
                SeatId = seat.Id,
                TicketNumber = GenerateTicketNumber(seat.Row,seat.Number),
                Status = CommConstants.CST_SEAT_STATUS_RESERVED,
                ReservedAt = DateTime.UtcNow
            }).ToList();

            await _context.Tickets.AddRangeAsync(tickets);
            await _context.SaveChangesAsync();

            return tickets;
        }
        private string GenerateTicketNumber(string row, int number)
        {
            string dateNow = DateTime.UtcNow.ToString("yyyyMMdd");
            string randomCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            return $"TICKET-{randomCode}-{dateNow}-{row}{number:D2}";
        }
    }
}
