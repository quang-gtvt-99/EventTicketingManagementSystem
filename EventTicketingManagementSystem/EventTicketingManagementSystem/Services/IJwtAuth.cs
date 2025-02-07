namespace EventTicketingManagementSystem.Services
{
    public interface IJwtAuth
    {
        string Authentication(string username, string password);
    }
}
