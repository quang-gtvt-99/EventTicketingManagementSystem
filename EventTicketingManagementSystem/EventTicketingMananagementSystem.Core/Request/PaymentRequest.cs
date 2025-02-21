namespace EventTicketingManagementSystem.API.Request
{
    public class PaymentRequest
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Locale { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string OrderType { get; set; } = string.Empty;
    }
}
