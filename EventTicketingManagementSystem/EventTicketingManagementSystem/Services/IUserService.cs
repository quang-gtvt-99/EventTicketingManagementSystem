
using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Request;
using EventTicketingManagementSystem.Response;

namespace EventTicketingManagementSystem.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<UserInfoDto> GetUserProfileAsync();
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    }
}
