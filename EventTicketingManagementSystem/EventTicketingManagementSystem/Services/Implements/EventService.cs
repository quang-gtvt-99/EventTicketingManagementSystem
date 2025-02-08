using EventTicketingManagementSystem.Data.Repository;
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

    }
}
