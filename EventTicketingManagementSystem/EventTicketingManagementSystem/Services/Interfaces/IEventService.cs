using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventById(int id);

        Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status);

        Task<Event> CreateEvent(Event eventItem);

        Task<bool> UpdateEvent(Event eventItem);

        Task<bool> DeleteEvent(Event eventItem);
    }
}
