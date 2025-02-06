namespace EventTicketingManagementSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiryDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public Event Event { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<BookingDetail> BookingDetails { get; set; }
        public ICollection<Payment> Payments { get; set; }
    }
}
