using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EventTicketingManagementSystem.Data.Data.Repository.Implement
{
    public class EventRepository : GenericRepository<Event, int>, IEventRepository
    {
        public EventRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => (e.Name != null && e.Name.Contains(search)) || (e.Description != null && e.Description.Contains(search)));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(e => e.Category == category);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(e => e.Status == status);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetFilteredPagedAsync(EventSearchParamsRequest eventFilter)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(eventFilter.Search))
            {
                query = query.Where(e => (e.Name != null && e.Name.ToLower().Contains(eventFilter.Search.ToLower())) || (e.Description != null && e.Description.ToLower().Contains(eventFilter.Search.ToLower())));
            }

            if (!string.IsNullOrEmpty(eventFilter.Category))
            {
                query = query.Where(e => e.Category == eventFilter.Category);
            }

            if (!string.IsNullOrEmpty(eventFilter.Status))
            {
                query = query.Where(e => e.Status == eventFilter.Status);
            }

            return await query.ToListAsync();
        }

        public async Task<int> CountSearch(string search)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => (e.Name != null && e.Name.ToLower().Contains(search.ToLower())) || (e.Description != null && e.Description.ToLower().Contains(search.ToLower())));
            }
            return await query.CountAsync();
        }
        public async Task<EventBookingInfoDto?> GetEventInfoWithSeatsByEventIDAsync(int eventId)
        {
            var eventInfo = await _context.Events
                .Where(e => e.Id == eventId)
                .Select(e => new EventBookingInfoDto
                {
                    EventInfo = new EventInfoDto
                    {
                        EventID = e.Id,
                        EventName = e.Name,
                        Description = e.Description,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        VenueName = e.VenueName,
                        VenueAddress = e.VenueAddress,
                        ImageUrls = e.ImageUrls,
                        TrailerUrls = e.TrailerUrls
                    },
                    SeatInfos = e.Seats
                    .OrderBy(s => s.Id)
                    .Select(s => new SeatInfoDto
                    {
                        EventId = s.EventId,
                        SeatId = s.Id,
                        Number = s.Number,
                        Row = s.Row,
                        Type = s.Type,
                        Price = s.Price,
                        Status = s.Status
                    })
                    .ToList()
                })
                .FirstOrDefaultAsync();

            return eventInfo;
        }

        public async Task<(string Message, int TotalSeats)> RegisterSeatsForEventAsync(CreateSeatDto createSeatDto)
        {
            var seats = new List<Seat>();
            for (char row = CommConstants.CST_SEAT_ROW_START; row <= CommConstants.CST_SEAT_ROW_END; row++)
            {
                for (int number = CommConstants.CST_SEAT_NUM_START; number <= CommConstants.CST_SEAT_NUM_END; number++)
                {
                    bool isVip = "CDEFGH".Contains(row) && number >= CommConstants.CST_SEAT_NUM_START + 2 && number <= CommConstants.CST_SEAT_NUM_END - 2;
                    var seat = new Seat
                    {
                        EventId = createSeatDto.EventId,
                        Row = row.ToString(),
                        Number = number,
                        Type = isVip ? CommConstants.CST_SEAT_TYPE_VIP : CommConstants.CST_SEAT_TYPE_NOR,
                        Price = isVip ? (int)Math.Ceiling(createSeatDto.Price * 1.2m) : createSeatDto.Price,
                        Status = CommConstants.CST_SEAT_STATUS_DEFAULT
                    };
                    seats.Add(seat);
                }
            }
            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();

            return ("registed successfully.", seats.Count);
        }

        public async Task<bool> UpdateSeatsPriceForEventAsync(CreateSeatDto updateSeatDto)
        {
            var seats = await _context.Seats.Where(seat => seat.EventId == updateSeatDto.EventId).ToListAsync();
            seats.ForEach(seat =>
            {
                bool isVip = "CDEFGH".Contains(seat.Row)
                    && seat.Number >= CommConstants.CST_SEAT_NUM_START + 2
                    && seat.Number <= CommConstants.CST_SEAT_NUM_END - 2;
                seat.Price = isVip ? (int)Math.Ceiling(updateSeatDto.Price * 1.2m) : updateSeatDto.Price;

            });
            _context.Seats.UpdateRange(seats);
            var res = await _context.SaveChangesAsync() == seats.Count();

            return res;
        }

        public async Task<List<Event>> GetUpcomingEventsAsync()
        {
            return await _context.Events
                .AsNoTracking()
                .Where(e => e.StartDate > DateTime.UtcNow)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }
    }
}
