using EventTicketingManagementSystem.Data;
using EventTicketingManagementSystem.Data.Repository;
using EventTicketingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace EventTicketingManagementSystem.Services
{
    public class AppDbService : IAppDbService
    {
        private readonly AppDbContext _context;

        public AppDbService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<IEnumerable<UserRole>> GetAllUserRolesAsync()
        {
            return await _context.UserRoles.ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<IEnumerable<TicketType>> GetAllTicketTypesAsync()
        {
            return await _context.TicketTypes.ToListAsync();
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await _context.Tickets.ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<IEnumerable<BookingDetail>> GetAllBookingDetailsAsync()
        {
            return await _context.BookingDetails.ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetAllPaymentsAsync()
        {
            return await _context.Payments.ToListAsync();
        }
    }

}
