using EventTicketingMananagementSystem.Core.Models.BaseModels;

namespace EventTicketingMananagementSystem.Core.Models
{
    public class Ticket : EntityAuditBase<int>
    {
        public int BookingId { get; set; }
        public int SeatId { get; set; }
        public string TicketNumber { get; set; }
        public string Status { get; set; }
        public DateTime? ReservedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
        public Seat Seat { get; set; }
    }
}
