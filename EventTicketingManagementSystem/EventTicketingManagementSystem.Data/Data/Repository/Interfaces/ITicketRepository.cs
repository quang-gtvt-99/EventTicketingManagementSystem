using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Data.Data.Repository.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket, int>
    {
        Task<List<Ticket>> CreateTicketsAsync(int bookingId);
    }
}
