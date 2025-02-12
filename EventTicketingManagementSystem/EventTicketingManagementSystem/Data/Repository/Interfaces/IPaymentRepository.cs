using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Data.Repository.Interfaces
{
    public interface IPaymentRepository:IGenericRepository<Payment, int>
    {
        Task<Payment> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentDto requestDto);
        Task<bool> DeleteExpiredBookingAsync(int paymentId);
    }
}
