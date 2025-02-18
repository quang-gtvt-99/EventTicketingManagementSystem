namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class UserInfoDto
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public List<BookingInfoDto> Bookings { get; set; } = new List<BookingInfoDto>();
    }
}
