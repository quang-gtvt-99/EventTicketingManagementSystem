using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingMananagementSystem.Core.Dtos;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventTicketingManagementSystem.Data.Data.Repository.Implement
{
    public class RoleRepository : GenericRepository<Role, int>, IRoleRepository
    {
        private readonly ILogger<RoleRepository> _logger;
        public RoleRepository(
            AppDbContext context,
            ILogger<RoleRepository> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<RoleDto?> GetRoleByName(string name)
        {
            var listRoleCache = await GetRoles();
            return listRoleCache?.FirstOrDefault(r => r.Name == name);
        }

        public async Task<RoleDto?> GetRoleById(int id)
        {
            var listRoleCache = await GetRoles();
            return listRoleCache?.FirstOrDefault(r => r.Id == id);
        }

        public async Task<List<RoleDto>> GetRoleByIds(List<int> ids)
        {
            var listRoleCache = await GetRoles();
            return listRoleCache?.Where(r => ids.Contains(r.Id)).ToList() ?? new List<RoleDto>();
        }

        private async Task<List<RoleDto>?> GetRoles()
        {
            var roleDtos = await _dbSet
                .Select(x => new RoleDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .ToListAsync();

            return roleDtos;
        }
    }
}
