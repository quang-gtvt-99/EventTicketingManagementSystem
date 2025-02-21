namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class UpdatePaymentDto
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public DateTime? PaymentDate { get; set; }
    }
}
