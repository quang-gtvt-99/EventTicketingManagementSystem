using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Response
{
    public class AuthResult
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public List<string> Roles { get; set; }
    }
}
