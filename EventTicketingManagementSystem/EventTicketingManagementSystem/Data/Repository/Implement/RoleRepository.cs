using EventTicketingManagementSystem.Constants;
using EventTicketingManagementSystem.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Repository.Implement
{
    public class RoleRepository : GenericRepository<Role, int>, IRoleRepository
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<RoleRepository> _logger;
        public RoleRepository(
            AppDbContext context,
            ICacheService cacheService,
            ILogger<RoleRepository> logger) : base(context)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<RoleDto?> GetRoleByName(string name)
        {
            var listRoleCache = await GetRolesFromCache();
            return listRoleCache?.FirstOrDefault(r => r.Name == name);
        }

        public async Task<RoleDto?> GetRoleById(int id)
        {
            var listRoleCache = await GetRolesFromCache();
            return listRoleCache?.FirstOrDefault(r => r.Id == id);
        }

        public async Task<List<RoleDto>> GetRoleByIds(List<int> ids)
        {
            var listRoleCache = await GetRolesFromCache();
            return listRoleCache?.Where(r => ids.Contains(r.Id)).ToList() ?? new List<RoleDto>();
        }

        private async Task<List<RoleDto>?> GetRolesFromCache()
        {
            var listRoleCache = await _cacheService.GetAsync<List<RoleDto>>(CacheKeyConsts.Roles);

            if (listRoleCache == null)
            {
                var roleDtos = await _dbSet
                    .Select(x => new RoleDto()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description
                    })
                    .ToListAsync();

                var setCacheResult = await _cacheService.SetAsync(CacheKeyConsts.Roles, roleDtos, 60 * 60);

                if (!setCacheResult)
                {
                    _logger.LogError("Set cache failed.");
                }

                return roleDtos;
            }

            return listRoleCache;
        }
    }
}
