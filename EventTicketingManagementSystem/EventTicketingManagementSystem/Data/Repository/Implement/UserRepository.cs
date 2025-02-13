using EventTicketingManagementSystem.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Enums;
using EventTicketingManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Repository.Implement
{
    public class UserRepository : GenericRepository<User, int>, IUserRepository 
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<List<string>> GetUserRolesAsync(int userId)
        {
            return await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                .ToListAsync();
        }

        public async Task AssignRoleAsync(int userId, UserRoles roleId)
        {
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = (int)roleId,
                AssignedAt = DateTime.UtcNow
            };

            await _context.UserRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

    }
}
