using EventTicketingManagementSystem.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Enums;
using EventTicketingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Repository.Implement
{
    public class UserRepository : GenericRepository<User, int>, IUserRepository 
    {
        private readonly IRoleRepository _roleRepository;
        public UserRepository(AppDbContext context, IRoleRepository roleRepository) : base(context)
        {
            _roleRepository = roleRepository;
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email.ToLower());
        }

        public async Task<bool> UserEmailExisted(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email.ToLower());
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            var roleIds = await _context.UserRoles
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var roles = await _roleRepository.GetRoleByIds(roleIds);

            return roles.Select(x => x.Name).ToList();
        }

        public async Task AssignRoleAsync(int userId, string roleName)
        {
            var role = await _roleRepository.GetRoleByName(roleName);

            if (role == null)
            {
                throw new Exception($"Role {roleName} not found.");
            }

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = role.Id,
                AssignedAt = DateTime.UtcNow
            };

            await _context.UserRoles.AddAsync(userRole);
        }

    }
}
