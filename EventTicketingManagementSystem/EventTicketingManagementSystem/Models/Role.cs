using EventTicketingManagementSystem.Models.BaseModels;

namespace EventTicketingManagementSystem.Models
{
    public class Role : EntityAuditBase<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
