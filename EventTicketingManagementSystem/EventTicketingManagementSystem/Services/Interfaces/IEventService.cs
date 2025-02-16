using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Request;

namespace EventTicketingManagementSystem.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventById(int id);

        Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status);

        Task<Event> CreateEvent(AddUpdateEventRequest eventItem);

        Task<bool> UpdateEvent(AddUpdateEventRequest eventItem);

        Task<IEnumerable<Event>> GetFilteredPagedEventsAsync(EventSearchParamsRequest eventFilter);
        Task<bool> DeleteEvent(int id);

        ///user///
        Task<IEnumerable<Event>> GetAllEventAsync();
        Task<Event?> GetEventDetailByIdAsync(int id);
        Task<EventBookingInfoDto> GetEventInfoWithSeatAsync(int id);
        Task<(string Message, int TotalSeats)> RegisterSeats(CreateSeatDto createSeatDto);
        Task<bool> UpdateSeatAsync(int eventId, string row, int number, UpdateSeatDto updateSeatDto);
        Task<bool> UpdateSeatAsync(int seatId, UpdateSeatDto updateSeatDto);

        ///

    }
}
