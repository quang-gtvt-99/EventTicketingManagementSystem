using EventTicketingManagementSystem.Data.Repository;
using EventTicketingManagementSystem.Models;
using EventTicketingManagementSystem.Request;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EventTicketingManagementSystem.Services.Implements
{
    public class JwtAuth : IJwtAuth
    {
        private readonly string _secretKey;
        private readonly IUserRepository _userRepository;

        public JwtAuth(string secretKey, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _secretKey = secretKey;
        }

        public string Authentication(string email, string password)
        {
            var user = _userRepository.FindByEmailAsync(email).Result;

            if (user == null)
            {
                return null;
            }

            if (!VerifyPasswordHash(password, user.PasswordHash))
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

            var roles = _userRepository.GetUserRolesAsync(user.Id).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1), 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }

    }
}