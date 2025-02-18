using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingManagementSystem.Data.Data.Repository.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role, int>
    {
        Task<RoleDto?> GetRoleById(int id);
        Task<List<RoleDto>> GetRoleByIds(List<int> ids);
        Task<RoleDto?> GetRoleByName(string name);
    }
}
