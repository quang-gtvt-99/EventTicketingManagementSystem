namespace EventTicketingManagementSystem.Request
{
    public class PaymentRequest
    {
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Locale { get; set; }
        public string BankCode { get; set; }
        public string OrderType { get; set; }
    }
}
