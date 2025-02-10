using EventTicketingManagementSystem.Data.Repository;
using EventTicketingManagementSystem.Dtos;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Request;
using EventTicketingManagementSystem.Response;
using EventTicketingManagementSystem.Services.Interfaces;

namespace EventTicketingManagementSystem.Services.Implements
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;

        public UserService(IUserRepository userRepository, IBookingRepository bookingRepository)
        {
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<UserInfoDto> GetUserProfileAsync()
        {
            // TODO: Get current user
            var user = new User()
            {
                Id = 1,
                FullName = "John Doe",
                Email = "test@email.com",
                PhoneNumber = "1234567890",
            };

            var bookings = await _bookingRepository.GetBookingInfosByUserIdAsync(user.Id);

            return new UserInfoDto()
            {
                Bookings = bookings,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName,
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                throw new Exception("Email already exists.");
            }

            var user = new User
            {
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Status = "Active"
            };
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            await _userRepository.AddAsync(user);

            await _userRepository.AssignRoleAsync(user.Id, "User");

            return new RegisterResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName
            };
        }

    }
}
