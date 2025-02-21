using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Data.Data.Repository.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking, int>
    {
        Task<List<BookingInfoDto>> GetBookingInfosByUserIdAsync(int userId);
        Task<Booking> CreateBookingAsync(CreateBookingDto bookingRequestDto, int loggedInUserId);
        Task<IEnumerable<Booking>> GetPendingExpiredBookingsAsync();
        Task<bool> DeleteBookingByIdAsync(int bookingId);
    }
}
