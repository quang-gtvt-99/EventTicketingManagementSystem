using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Data.Repository.Interfaces
{
    public interface IRoleRepository : IGenericRepository<Role, int>
    {
        Task<RoleDto?> GetRoleById(int id);
        Task<List<RoleDto>> GetRoleByIds(List<int> ids);
        Task<RoleDto?> GetRoleByName(string name);
    }
}
