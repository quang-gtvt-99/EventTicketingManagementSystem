using EventTicketingManagementSystem.Models.BaseModels;

namespace EventTicketingManagementSystem.Models
{
    public class Seat : EntityAuditBase<int>
    {
        public int EventId { get; set; }
        public string Row { get; set; }
        public int Number { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }

        // Navigation properties
        public Event Event { get; set; }
        public Ticket? Ticket { get; set; }
    }
}
