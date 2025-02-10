using EventTicketingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Repository.Implement
{
    public class EventRepository: GenericRepository<Event, int>, IEventRepository
    {
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
    }
}
