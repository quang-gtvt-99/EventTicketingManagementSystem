using EventTicketingManagementSystem.Models;
using Persistence.Repositories.Interfaces.Generic;

namespace EventTicketingManagementSystem.Data.Repository
{
    public interface IUserRepository : IGenericRepository<User, int>
    {
        Task<User> FindByEmailAsync(string email);

        Task<List<string>> GetUserRolesAsync(int userId);

        Task AssignRoleAsync(int userId, string roleName);
    }
}
