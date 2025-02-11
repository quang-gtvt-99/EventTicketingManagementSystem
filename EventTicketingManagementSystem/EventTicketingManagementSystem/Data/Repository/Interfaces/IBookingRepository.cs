using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Data.Repository.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking, int>
    {
        Task<List<BookingInfoDto>> GetBookingInfosByUserIdAsync(int userId);
    }
}
