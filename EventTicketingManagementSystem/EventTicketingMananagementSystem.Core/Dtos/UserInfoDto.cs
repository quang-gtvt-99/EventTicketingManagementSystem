using EventTicketingMananagementSystem.Core.Enums;

namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class UserInfoDto
    {
        public int? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool? IsActive { get; set; }
        public UserRoles? Role { get; set; }
        public List<BookingInfoDto> Bookings { get; set; } = new List<BookingInfoDto>();
    }
}
