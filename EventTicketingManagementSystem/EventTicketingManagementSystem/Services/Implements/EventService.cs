using EventTicketingManagementSystem.Data.Repository;
using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Services.Interfaces;

namespace EventTicketingManagementSystem.Services.Implements
{
    public class EventService : IEventService
    {

        private readonly IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllAsync();
        }

        public async Task<Event> GetEventById(int id) => await _eventRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status) =>
            await _eventRepository.GetEventsByFilter(search, category, status);

        public async Task<Event> CreateEvent(Event eventItem) => await _eventRepository.AddAsync(eventItem);

        public async Task<bool> UpdateEvent(Event eventItem) => await _eventRepository.UpdateAsync(eventItem);

        public async Task<bool> DeleteEvent(Event eventItem) => await _eventRepository.DeleteAsync(eventItem);


        ///user
        public async Task<(string Message, int TotalSeats)> RegisterSeats(CreateSeatDto createSeatDto)
        {
            return await _eventRepository.RegisterSeatsForEventAsync(createSeatDto);
        }
        public async Task<IEnumerable<Event>> GetAllEventAsync()
        {
            return await _eventRepository.GetAllAsync();
        }
        public async Task<Event?> GetEventDetailByIdAsync(int id)
        {
            return await _eventRepository.GetByIdAsync(id);
        }
        public async Task<EventBookingInfoDto> GetEventInfoWithSeatAsync(int id)
        {
            return await _eventRepository.GetEventInfoWithSeatsByEventIDAsync(id);
        }
        public async Task<bool> UpdateSeatAsync(int eventId, string row, int number, UpdateSeatDto updateSeatDto)
        {
            return await _eventRepository.UpdateSeatByEventIdRowSeatNameAsync(eventId, row, number, updateSeatDto);
        }
        public async Task<bool> UpdateSeatAsync(int seatId, UpdateSeatDto updateSeatDto)
        {
            return await _eventRepository.UpdateSeatBySeatIdAsync(seatId, updateSeatDto);
        }
        ///
    }
}
