using EventTicketingMananagementSystem.Core.Enums;

namespace EventTicketingManagementSystem.API.Request
{
    public class AddUpdateUserRequest
    {
        public int? ID { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        public UserRoles? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}