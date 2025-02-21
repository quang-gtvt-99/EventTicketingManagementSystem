using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Data.Data.Repository.Interfaces
{
    public interface IUserRepository : IGenericRepository<User, int>
    {
        Task<User?> FindByEmailAsync(string email);

        Task<List<string>> GetUserRolesAsync(int userId);

        Task AssignRoleAsync(int userId, string roleName);
        Task<bool> UserEmailExisted(string email);
        string? GetEmailByIdAsync(int userId);
        Task<IEnumerable<User>> GetFilteredPagedAsync(string search);
    }
}
