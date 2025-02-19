
namespace EventTicketingManagementSystem.Services.Services.Interfaces
{
    public interface IBookingService
    {
        Task RemovePendingExpiredBookings();
    }
}
