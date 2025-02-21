using EventTicketingMananagementSystem.Core.Constants;
using EventTicketingMananagementSystem.Core.Models.BaseModels;

namespace EventTicketingMananagementSystem.Core.Models
{
    public class Seat : EntityAuditBase<int>
    {
        public int EventId { get; set; }
        public int? BookingId { get; set; }
        public string Row { get; set; } = string.Empty;
        public int Number { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = CommConstants.CST_SEAT_STATUS_DEFAULT;

        // Navigation properties
        public Event Event { get; set; } = default!;
        public Ticket? Ticket { get; set; }
        public Booking? Booking { get; set; }
    }
}
