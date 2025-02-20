using EventTicketingMananagementSystem.Core.Models.BaseModels;

namespace EventTicketingMananagementSystem.Core.Models
{
    public class Ticket : EntityAuditBase<int>
    {
        public int BookingId { get; set; }
        public int SeatId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? ReservedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        // Navigation properties
        public Booking Booking { get; set; } = default!;
        public Seat Seat { get; set; } = default!;
    }
}
