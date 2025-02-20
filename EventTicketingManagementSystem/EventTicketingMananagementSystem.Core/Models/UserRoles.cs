using EventTicketingMananagementSystem.Core.Models.BaseModels;

namespace EventTicketingMananagementSystem.Core.Models
{
    public class UserRole : EntityBase<int>
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public int? AssignedBy { get; set; }

        // Navigation properties
        public User User { get; set; } = default!;
        public Role Role { get; set; } = default!;
    }
}
