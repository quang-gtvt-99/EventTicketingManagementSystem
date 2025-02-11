using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Data.Repository.Interfaces
{
    public interface IUserRepository : IGenericRepository<User, int>
    {
        Task<User?> FindByEmailAsync(string email);

        Task<List<string>> GetUserRolesAsync(int userId);

        Task AssignRoleAsync(int userId, string roleName);
    }
}
