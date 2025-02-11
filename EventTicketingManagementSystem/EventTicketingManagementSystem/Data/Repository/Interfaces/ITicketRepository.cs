using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Data.Repository.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket, int>
    {
        Task<List<Ticket>> CreateTicketsAsync(int bookingId);
    }
}
