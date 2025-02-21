﻿using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingMananagementSystem.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingManagementSystem.Data.Data.Repository.Implement
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

        public string? GetEmailByIdAsync(int userId)
        {
             var email =  _context.Users
                .Where(e => e.Id == userId)
                .Select(e => e.Email)
                .FirstOrDefault();
            return email;
        }

        public async Task<IEnumerable<User>> GetFilteredPagedAsync(string? search)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.FullName.ToLower().Contains(search) || e.Email.ToLower().Contains(search));
            }

            return await query.ToListAsync();
        }
    }
}
