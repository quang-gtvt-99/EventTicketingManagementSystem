using EventTicketingManagementSystem.Data.Repository;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Services.Interfaces;

namespace EventTicketingManagementSystem.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }
    }
}
