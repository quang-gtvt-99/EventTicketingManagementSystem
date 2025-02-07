using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using Persistence.Repositories.Interfaces.Generic;

namespace EventTicketingManagementSystem.Data.Repository
{
    public interface IBookingRepository : IGenericRepository<Booking, int>
    {
        Task<List<BookingInfoDto>> GetBookingInfosByUserIdAsync(int userId);
    }
}
