using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Data.Data.Repository.Implement
{
    public class SeatRepository : GenericRepository<Seat, int>, ISeatRepository
    {
        public SeatRepository(AppDbContext context) : base(context)
        {
        }
    }
}
