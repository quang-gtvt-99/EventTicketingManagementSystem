using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using Persistence.Repositories.Interfaces.Generic;

namespace EventTicketingManagementSystem.Data.Repository
{
    public interface IEventRepository : IGenericRepository<Event, int>
    {
        Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status);

        ///user/
        Task<List<EventInfoDto>> GetEventInfoWithSeatsByEventIDAsync(int Id);
        Task<(string Message, int TotalSeats)> RegisterSeatsForEventAsync(CreateSeatDto createSeatDto);
        Task<bool> UpdateSeatBySeatIdAsync(int seatId, UpdateSeatDto updateSeatDto);
        Task<bool> UpdateSeatByEventIdRowSeatNameAsync(int eventId, string row, int number, UpdateSeatDto updateSeatDto);

        ///
    }
}
