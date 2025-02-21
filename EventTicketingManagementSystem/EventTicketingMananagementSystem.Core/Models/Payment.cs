using EventTicketingMananagementSystem.Core.Models.BaseModels;

namespace EventTicketingMananagementSystem.Core.Models
{
    public class Payment : EntityAuditBase<int>
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public string TransactionId { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
        public DateTime? RefundDate { get; set; }

        // Navigation properties
        public Booking Booking { get; set; } = default!;
    }
}
