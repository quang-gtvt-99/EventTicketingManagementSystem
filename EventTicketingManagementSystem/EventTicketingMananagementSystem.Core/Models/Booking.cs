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
        public User User { get; set; } = default!;
        public Event Event { get; set; } = default!;
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
