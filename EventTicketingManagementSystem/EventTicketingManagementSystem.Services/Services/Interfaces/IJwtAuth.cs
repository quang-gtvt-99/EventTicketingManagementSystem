using EventTicketingManagementSystem.API.Response;

namespace EventTicketingManagementSystem.Services.Services.Interfaces
{
    public interface IJwtAuth
    {
        Task<AuthResult?> Authentication(string email, string password);
    }
}
