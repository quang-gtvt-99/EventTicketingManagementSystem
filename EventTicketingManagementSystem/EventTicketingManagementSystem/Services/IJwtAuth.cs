namespace EventTicketingManagementSystem.Services
{
    public interface IJwtAuth
    {
        string Authentication(string email, string password);
    }
}
