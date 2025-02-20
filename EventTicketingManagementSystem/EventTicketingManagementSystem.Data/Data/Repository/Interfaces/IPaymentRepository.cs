using EventTicketingManagementSystem.Response;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Data.Data.Repository.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment, int>
    {
        Task<Payment> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentDto requestDto);
        Task<bool> DeleteExpiredBookingAsync(int paymentId);
        Task<Payment> CreatePaymentAsync(PaymentResponse request);
    }
}
