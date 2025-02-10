using EventTicketingManagementSystem.Models;
using Persistence.Repositories.Interfaces.Generic;

namespace EventTicketingManagementSystem.Data.Repository
{
    public interface IEventRepository : IGenericRepository<Event, int>
    {
        Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status);
    }
}
