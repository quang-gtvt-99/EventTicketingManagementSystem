
using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
