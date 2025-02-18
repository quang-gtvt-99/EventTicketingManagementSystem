namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class UpdatePaymentDto
    {
        public int BookingId { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}
