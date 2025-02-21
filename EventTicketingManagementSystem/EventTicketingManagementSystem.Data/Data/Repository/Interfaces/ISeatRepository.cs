using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Data.Data.Repository.Interfaces
{
    public interface ISeatRepository : IGenericRepository<Seat, int>
    {
        Task<List<Seat>> UpdateSeatsByBookingIdAsync(int bookingId, string status);
    }
}
