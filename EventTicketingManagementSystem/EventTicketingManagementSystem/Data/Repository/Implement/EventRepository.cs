using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventTicketingManagementSystem.Data.Repository.Implement
{
    public class EventRepository : GenericRepository<Event, int>, IEventRepository
    {
        private const string CST_SEAT_STATUS_DEFAULT = "available";
        private const int CST_SEAT_NUM_START = 1;
        private const int CST_SEAT_NUM_END = 16;
        private const char CST_SEAT_ROW_START = 'A';
        private const char CST_SEAT_ROW_END = 'J';
        public EventRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Event>> GetEventsByFilter(string search, string category, string status)
        {
            var query = _context.Events.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.Name.Contains(search) || e.Description.Contains(search));
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
        ///////user
        public async Task<EventBookingInfoDto> GetEventInfoWithSeatsByEventIDAsync(int eventId)
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
                        ImageUrls = e.ImageUrls 
                    },
                    SeatInfos = e.Seats.Select(s => new SeatInfoDto
                    {
                        EventId = s.EventId,
                        SeatId = s.Id,
                        Number = s.Number,
                        Row = s.Row,
                        Type = s.Type,
                        Price = s.Price,
                        Status = s.Status
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return eventInfo ?? new EventBookingInfoDto();
        }

        public async Task<(string Message, int TotalSeats)> RegisterSeatsForEventAsync(CreateSeatDto createSeatDto)
        {
            var seats = new List<Seat>();
            for (char row = CST_SEAT_ROW_START; row <= CST_SEAT_ROW_END; row++)
            {
                for (int number = CST_SEAT_NUM_START; number <= CST_SEAT_NUM_END; number++)
                {
                    bool isVip = "CDEFGH".Contains(row) && number >= CST_SEAT_NUM_START + 2 && number <= CST_SEAT_NUM_END - 2;
                    var seat = new Seat
                    {
                        EventId = createSeatDto.EventId,
                        Row = row.ToString(),
                        Number = number,
                        Type = isVip ? "vip" : "normal",
                        Price = isVip ? createSeatDto.Price * 1.2m : createSeatDto.Price,
                        Status = CST_SEAT_STATUS_DEFAULT
                    };
                    seats.Add(seat);
                }
            }
            _context.Seats.AddRange(seats);
            await _context.SaveChangesAsync();

            return ("registed successfully.", seats.Count);
        }
        public async Task<bool> UpdateSeatBySeatIdAsync(int seatId, UpdateSeatDto updateSeatDto)
        {
            var seat = await _context.Seats.FindAsync(seatId);
            if (seat == null)
                return false;

            if (updateSeatDto.Price > 0)
            {
                seat.Price = updateSeatDto.Price;

                if (seat.Type?.ToLower() == "vip")
                {
                    seat.Price *= 1.2m;
                }
            }

            if (!string.IsNullOrEmpty(updateSeatDto.Status))
                seat.Status = updateSeatDto.Status;

            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSeatByEventIdRowSeatNameAsync(int eventId, string row, int number, UpdateSeatDto updateSeatDto)
        {
            var seat = await _context.Seats.FirstOrDefaultAsync(s =>
                s.EventId == eventId &&
                s.Row == row &&
                s.Number == number);

            if (seat == null)
                return false;

            if (updateSeatDto.Price > 0)
            {
                seat.Price = updateSeatDto.Price;

                if (seat.Type?.ToLower() == "vip")
                {
                    seat.Price *= 1.2m;
                }
            }

            if (!string.IsNullOrEmpty(updateSeatDto.Status))
                seat.Status = updateSeatDto.Status;

            _context.Seats.Update(seat);
            await _context.SaveChangesAsync();
            return true;
        }
        ///
    }
}
