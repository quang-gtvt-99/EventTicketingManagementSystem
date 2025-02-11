namespace EventTicketingManagementSystem.Services.Interfaces
{
    public interface ICurrentUserService
    {
        string? Email { get; set; }
        string? Id { get; set; }
        string? FullName { get; set; }
    }
}
