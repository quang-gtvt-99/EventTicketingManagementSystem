using EventTicketingManagementSystem.Data.Repository;
using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Request;
using EventTicketingManagementSystem.Services.Interfaces;
using EventTicketingManagementSystem.Share;
using Microsoft.AspNetCore.Components;

namespace EventTicketingManagementSystem.Services.Implements
{
    public class EventService : IEventService
    {
        [Inject] IObjectStorageService objectStorageService { get; set; }

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

        public async Task<Event> CreateEvent(AddUpdateEventRequest eventItem)
        {
            try
            {

                var imageUrl = string.Empty;
                if (eventItem.Image != null)
                {
                    using (var fileStream = eventItem.Image.OpenReadStream())
                    {
                        // Generate the file name for S3 (you can modify this to meet your needs)
                        var fileName = $"{Guid.NewGuid()}_{eventItem.Image.FileName}";

                        // Call the UploadFileAsync method
                        imageUrl = await objectStorageService.UploadFileAsync(fileStream, Guid.NewGuid().ToString(), CommonConst.S3_BUCKET_NAME);
                    }
                }
                var eventObj = new Event
                {
                    Name = eventItem.Name ?? string.Empty,
                    Description = eventItem.Description ?? string.Empty,
                    StartDate = eventItem.StartDate.GetValueOrDefault(),
                    EndDate = eventItem.EndDate.GetValueOrDefault(),
                    VenueName = eventItem.VenueName,
                    VenueAddress = eventItem.VenueAddress,
                    ImageUrls = imageUrl,
                    Category = eventItem.Category?.ToString(),
                    SeatPrice = eventItem.SeatPrice
                };
                var eventCreated = await _eventRepository.AddAsync(eventObj);

                return eventCreated;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message);
                throw;
            }

        }

        public async Task<bool> UpdateEvent(AddUpdateEventRequest eventItem)
        {
            using var stream = eventItem.Image?.OpenReadStream();
            var imageUrl = string.Empty;
            if (stream != null)
            {
                imageUrl = await objectStorageService.UploadFileAsync(stream, new Guid().ToString(), CommonConst.S3_BUCKET_NAME);
            }

            var eventObj = await _eventRepository.GetByIdAsync(eventItem.ID);
            if (eventObj == null)
            {
                return false;
            }

            eventObj.Name = eventItem.Name ?? string.Empty;
            eventObj.Description = eventItem.Description ?? string.Empty;
            eventObj.StartDate = eventItem.StartDate.GetValueOrDefault();
            eventObj.EndDate = eventItem.EndDate.GetValueOrDefault();
            eventObj.VenueName = eventItem.VenueName ?? string.Empty;
            eventObj.ImageUrls = imageUrl;
            eventObj.VenueAddress = eventItem.VenueAddress ?? string.Empty;
            eventObj.Category = eventItem.Category?.ToString() ?? string.Empty;
            eventObj.SeatPrice = eventItem.SeatPrice.GetValueOrDefault();

            var isUpdated = await _eventRepository.UpdateAsync(eventObj);

            return isUpdated;
        }

        public async Task<bool> DeleteEvent(Event eventItem) => await _eventRepository.DeleteAsync(eventItem);

        public async Task<(IEnumerable<Event>, int)> GetFilteredPagedEventsAsync(EventSearchParamsRequest eventFilter)
        {
            var countTotal = await _eventRepository.CountSearch(eventFilter.Search);
            var events = await _eventRepository.GetFilteredPagedAsync(eventFilter);
            return (events, countTotal);
        }




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
