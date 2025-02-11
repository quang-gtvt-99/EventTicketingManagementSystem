using EventTicketingManagementSystem.Dtos;
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
