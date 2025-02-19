using EventTicketingMananagementSystem.Core.Models.BaseModels;

namespace EventTicketingMananagementSystem.Core.Models
{
    public class Booking : EntityAuditBase<int>
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Event Event { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Seat> Seats { get; set; }
    }
}
