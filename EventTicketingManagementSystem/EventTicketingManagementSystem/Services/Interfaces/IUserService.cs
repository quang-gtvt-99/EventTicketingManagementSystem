using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Request;
using EventTicketingManagementSystem.Response;

namespace EventTicketingManagementSystem.Services.Interfaces
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
    }
}
