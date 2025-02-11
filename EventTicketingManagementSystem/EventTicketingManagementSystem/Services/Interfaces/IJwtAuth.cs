using EventTicketingManagementSystem.Response;

namespace EventTicketingManagementSystem.Services.Interfaces
{
    public interface IJwtAuth
    {
        AuthResult Authentication(string email, string password);
    }
}
