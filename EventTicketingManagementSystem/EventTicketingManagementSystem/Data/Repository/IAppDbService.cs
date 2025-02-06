using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Data.Repository
{
    public interface IAppDbService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<UserRole>> GetAllUserRolesAsync();
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<IEnumerable<TicketType>> GetAllTicketTypesAsync();
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<IEnumerable<BookingDetail>> GetAllBookingDetailsAsync();
        Task<IEnumerable<Payment>> GetAllPaymentsAsync();
    }
}
