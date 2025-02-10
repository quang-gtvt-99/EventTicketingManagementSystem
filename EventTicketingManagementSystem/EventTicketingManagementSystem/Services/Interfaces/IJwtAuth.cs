namespace EventTicketingManagementSystem.Services.Interfaces
{
    public interface IJwtAuth
    {
        string Authentication(string email, string password);
    }
}
