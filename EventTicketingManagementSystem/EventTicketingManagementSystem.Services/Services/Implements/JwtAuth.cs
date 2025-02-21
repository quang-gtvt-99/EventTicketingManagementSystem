using EventTicketingManagementSystem.API.Response;
using EventTicketingManagementSystem.Data.Data.Repository.Interfaces;
using EventTicketingManagementSystem.Services.Services.Interfaces;
using EventTicketingMananagementSystem.Core.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventTicketingManagementSystem.Services.Services.Implements
{
    public class JwtAuth : IJwtAuth
    {
        private readonly string _secretKey;
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;


        public JwtAuth(IUserRepository userRepository, ICacheService cacheService, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _secretKey = configuration["JwtKey"] ?? throw new Exception("Cannot get JwtKey");
            _cacheService = cacheService;
        }

        public async Task<AuthResult?> Authentication(string email, string password)
        {
            var user = await _userRepository.FindByEmailAsync(email);

            if (user == null && user.Status.Equals(CommConstants.CST_USER_STATUS_INACTIVE))
            {
                return null;
            }

            string? otpCache = null;
            if (await _cacheService.IsCacheKeyExistAsync($"{CacheKeyConsts.OneTimePassword}:{email}"))
            {
                otpCache = await _cacheService.GetAsync<string>($"{CacheKeyConsts.OneTimePassword}:{email}");
            }

            bool isPasswordValid = VerifyPasswordHash(password, user.PasswordHash);
            bool isOtpValid = otpCache != null && VerifyPasswordHash(password, otpCache);

            if (!isPasswordValid && !isOtpValid)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName)
            };

            var roles = await _userRepository.GetUserRolesAsync(user.Id);
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            user.LastLoginAt = DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangeAsync();

            return new AuthResult
            {
                Email = user.Email,
                Roles = roles ?? new List<string>(),
                Token = tokenString
            };
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

    }
}