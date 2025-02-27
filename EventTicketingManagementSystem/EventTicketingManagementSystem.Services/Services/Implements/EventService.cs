using Amazon.Runtime.Internal.Util;
using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.Extensions.Logging;

namespace EventTicketingManagementSystem.Services.Services.Implements
{
    public class EventService : IEventService
    {

        private readonly IEventRepository _eventRepository;
        private readonly IObjectStorageService _objectStorageService;
        private readonly ICacheService _cacheService;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepository, IObjectStorageService objectStorageService, ICacheService cacheService, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _objectStorageService = objectStorageService;
            _cacheService = cacheService;
            _logger = logger;
        }

        #region Admin Event
        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _eventRepository.GetAllAsync();
        }

        public async Task<Event?> GetEventById(int id) => await _eventRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status) =>
            await _eventRepository.GetEventsByFilter(search, category, status);

        public async Task<int> CreateEvent(AddUpdateEventRequest eventItem)
        {
            try
            {
                var imageUrl = string.Empty;
                if (eventItem.Image != null)
                {
                    using (var fileStream = eventItem.Image.OpenReadStream())
                    {
                        // Generate the file name for S3 (you can modify this to meet your needs)
                        var fileName = $"{Guid.NewGuid()}_{eventItem?.Image?.FileName}";
                        try
                        {
                            // Call the UploadFileAsync method
                            imageUrl = await _objectStorageService.UploadFileAsync(fileStream, fileName, CommConstants.S3_BUCKET_NAME);
                        }
                        catch (Exception)
                        {
                            imageUrl = string.Empty;
                        }
                    }
                }
                var eventObj = new Event
                {
                    Name = eventItem?.Name ?? string.Empty,
                    Description = eventItem?.Description ?? string.Empty,
                    StartDate = eventItem?.StartDate ?? DateTime.UtcNow,
                    EndDate = eventItem?.EndDate ?? DateTime.UtcNow,
                    VenueName = eventItem?.VenueName,
                    VenueAddress = eventItem?.VenueAddress,
                    ImageUrls = imageUrl,
                    Category = eventItem?.Category?.ToString(),
                    SeatPrice = eventItem?.SeatPrice,
                    TrailerUrls = eventItem?.TrailerUrls ?? string.Empty
                };
                var eventCreated = await _eventRepository.AddAsync(eventObj);
                await _eventRepository.SaveChangeAsync();
                // Invalid cache
                _logger.LogInformation("Invalid cache for upcoming events!");
                await _cacheService.InvalidCacheAsync(CacheKeyConsts.UPCOMING_EVENTS);

                var seatDto = new CreateSeatDto
                {
                    EventId = eventCreated.Id,
                    Price = eventItem?.SeatPrice ?? 0,
                };

                await _eventRepository.RegisterSeatsForEventAsync(seatDto);

                return eventCreated.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        public async Task<bool> UpdateEvent(AddUpdateEventRequest eventItem)
        {
            var currentEventItem = await _eventRepository.GetByIdAsync(eventItem.ID);

            if (currentEventItem == null)
            {
                return false;
            }

            using var fileStream = eventItem.Image?.OpenReadStream();

            var imageUrl = currentEventItem.ImageUrls;

            if (fileStream != null)
            {
                try
                {
                    // Call the UploadFileAsync method
                    var fileName = $"{Guid.NewGuid()}_{eventItem?.Image?.FileName}";
                    imageUrl = await _objectStorageService.UploadFileAsync(fileStream, fileName, CommConstants.S3_BUCKET_NAME);
                }
                catch (Exception)
                {
                    imageUrl = currentEventItem.ImageUrls;
                }
            }

            if (eventItem == null)
            {
                throw new ArgumentNullException(nameof(eventItem));
            }

            var eventObj = await _eventRepository.GetByIdAsync(eventItem.ID);
            if (eventObj == null)
            {
                return false;
            }
            var isChangePrice = eventObj.SeatPrice != eventItem.SeatPrice.GetValueOrDefault();
            eventObj.Name = eventItem.Name ?? string.Empty;
            eventObj.Description = eventItem.Description ?? string.Empty;
            eventObj.StartDate = eventItem.StartDate.GetValueOrDefault();
            eventObj.EndDate = eventItem.EndDate.GetValueOrDefault();
            eventObj.VenueName = eventItem.VenueName ?? string.Empty;
            eventObj.ImageUrls = imageUrl;
            eventObj.VenueAddress = eventItem.VenueAddress ?? string.Empty;
            eventObj.Category = eventItem.Category?.ToString() ?? string.Empty;
            eventObj.SeatPrice = eventItem.SeatPrice.GetValueOrDefault();
            eventObj.TrailerUrls = eventItem.TrailerUrls ?? string.Empty;

            _eventRepository.Update(eventObj);
            if (isChangePrice)
            {
                var seatDto = new CreateSeatDto
                {
                    EventId = eventObj.Id,
                    Price = eventItem?.SeatPrice ?? 0,
                };
                await _eventRepository.UpdateSeatsPriceForEventAsync(seatDto);
            }
            var isUpdated = await _eventRepository.SaveChangeAsync() > 0;
            // Invalid cache
            _logger.LogInformation($"Invalid cache for upcoming events and event detail {currentEventItem.Id}!");
            await _cacheService.InvalidCacheAsync(CacheKeyConsts.UPCOMING_EVENTS);
            await _cacheService.InvalidCacheAsync(CacheKeyConsts.EVENT_DETAIL + currentEventItem.Id);

            return isUpdated;
        }

        public async Task<bool> DeleteEvent(int id)
        {
            var eventItem = await _eventRepository.GetByIdAsync(id);

            if (eventItem == null)
            {
                return false;
            }
            // Invalid cache
            _logger.LogInformation($"Invalid cache for upcoming events and event detail {id}!");
            await _cacheService.InvalidCacheAsync(CacheKeyConsts.UPCOMING_EVENTS);
            await _cacheService.InvalidCacheAsync(CacheKeyConsts.EVENT_DETAIL + id);

            _eventRepository.Delete(eventItem);
            return await _eventRepository.SaveChangeAsync() > 0;
        }

        public async Task<IEnumerable<Event>> GetFilteredPagedEventsAsync(EventSearchParamsRequest eventFilter)
        {
            if (eventFilter.IsUpcoming)
            {
                return await GetUpcommingEvents();
            }

            var events = await _eventRepository.GetFilteredPagedAsync(eventFilter);

            return events;
        }

        private async Task<IEnumerable<Event>> GetUpcommingEvents()
        {
            List<Event> upcomingEvents = await _cacheService.GetAsync<List<Event>>(CacheKeyConsts.UPCOMING_EVENTS) ?? new List<Event>();

            if (upcomingEvents.Count != 0)
            {
                _logger.LogInformation("Get upcoming events in cache!");
                return upcomingEvents;
            }

            upcomingEvents = await _eventRepository.GetUpcomingEventsAsync();
            await _cacheService.SetAsync(CacheKeyConsts.UPCOMING_EVENTS, upcomingEvents, 60 * 10);
            return upcomingEvents;
        }
        #endregion

        #region User Event
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
            var eventItem = await _cacheService.GetAsync<Event>(CacheKeyConsts.EVENT_DETAIL + id);
            if (eventItem != null)
            {
                _logger.LogInformation($"Get detail for event {id} in cache!");
                return eventItem;
            }
            var eventDetail = await _eventRepository.GetByIdAsync(id);
            await _cacheService.SetAsync(CacheKeyConsts.EVENT_DETAIL + id, eventDetail);
            return eventDetail;
        }
        public async Task<EventBookingInfoDto?> GetEventInfoWithSeatAsync(int id)
        {
            return await _eventRepository.GetEventInfoWithSeatsByEventIDAsync(id);
        }
        #endregion
    }
}
