using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
