using EventTicketingManagementSystem.API.Request;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Data.Data.Repository.Interfaces
{
    public interface IEventRepository : IGenericRepository<Event, int>
    {
        Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status);
        Task<IEnumerable<Event>> GetFilteredPagedAsync(EventSearchParamsRequest eventFilter);
        Task<int> CountSearch(string search);
        ///user/
        Task<EventBookingInfoDto> GetEventInfoWithSeatsByEventIDAsync(int Id);
        Task<(string Message, int TotalSeats)> RegisterSeatsForEventAsync(CreateSeatDto createSeatDto);
        Task<bool> UpdateSeatBySeatIdAsync(int seatId, UpdateSeatDto updateSeatDto);
        Task<bool> UpdateSeatByEventIdRowSeatNameAsync(int eventId, string row, int number, UpdateSeatDto updateSeatDto);
        Task<List<Event>> GetUpcomingEventsAsync();
    }
}
