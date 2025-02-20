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
        Task<EventBookingInfoDto?> GetEventInfoWithSeatsByEventIDAsync(int Id);
        Task<(string Message, int TotalSeats)> RegisterSeatsForEventAsync(CreateSeatDto createSeatDto);
        Task<List<Event>> GetUpcomingEventsAsync();
    }
}
