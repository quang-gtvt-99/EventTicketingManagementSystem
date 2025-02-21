using EventTicketingManagementSystem.API.Request;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Services.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event?> GetEventById(int id);

        Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status);

        Task<int> CreateEvent(AddUpdateEventRequest eventItem);

        Task<bool> UpdateEvent(AddUpdateEventRequest eventItem);

        Task<IEnumerable<Event>> GetFilteredPagedEventsAsync(EventSearchParamsRequest eventFilter);
        Task<bool> DeleteEvent(int id);
        Task<IEnumerable<Event>> GetAllEventAsync();
        Task<Event?> GetEventDetailByIdAsync(int id);
        Task<EventBookingInfoDto?> GetEventInfoWithSeatAsync(int id);
        Task<(string Message, int TotalSeats)> RegisterSeats(CreateSeatDto createSeatDto);
    }
}
