namespace EventTicketingManagementSystem.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public int BookingId { get; set; }
        public int TicketTypeId { get; set; }
        public string TicketNumber { get; set; }
        public string Status { get; set; } = "Reserved";
        public DateTime? ReservedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Booking Booking { get; set; }
        public TicketType TicketType { get; set; }
    }
}
