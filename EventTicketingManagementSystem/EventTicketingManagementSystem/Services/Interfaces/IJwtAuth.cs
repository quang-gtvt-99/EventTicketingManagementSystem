using EventTicketingManagementSystem.Response;

namespace EventTicketingManagementSystem.Services.Interfaces
{
    public interface IJwtAuth
    {
        Task<AuthResult?> Authentication(string email, string password);
    }
}
