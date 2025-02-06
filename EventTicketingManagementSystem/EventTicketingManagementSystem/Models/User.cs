using Microsoft.Extensions.Logging;

namespace EventTicketingManagementSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string Status { get; set; } = "Active";

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Event> Events { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
