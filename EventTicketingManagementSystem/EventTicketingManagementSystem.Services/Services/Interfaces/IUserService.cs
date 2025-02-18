using EventTicketingManagementSystem.API.Request;
using EventTicketingManagementSystem.API.Response;
using EventTicketingManagementSystem.Response;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<UserInfoDto> GetUserProfileAsync();
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<Booking> CreateBookingAsync(CreateBookingDto bookingRequestDto, int loggedInUserId);
        Task<Payment> UpdatePaymentStatusAsync(int paymentId, UpdatePaymentDto requestDto);
        Task<bool> DeleteExpiredBookingAsync(int paymentId);
        Task<List<Ticket>> CreateTicketsAsync(int bookingId);
        Task<CommonMessageResponse> UpdateUserProfileAsync(UpdateUserProfileRequest request);
        Task<CommonMessageResponse> ResetPasswordAsync(ResetPasswordRequest request);
        Task SendEmailToId(PaymentResponse response,int userId);
    }
}
