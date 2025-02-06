namespace EventTicketingManagementSystem.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; } = "Pending";
        public string TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? RefundDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
    }
}
