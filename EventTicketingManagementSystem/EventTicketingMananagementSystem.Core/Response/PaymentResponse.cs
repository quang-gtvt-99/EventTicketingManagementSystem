namespace EventTicketingManagementSystem.Response
{
    public class PaymentResponse
    {
        public int BookingId { get; set; }
        public long VnPayTranId { get; set; }
        public string ResponseCode { get; set; }
        public string TransactionStatus { get; set; }
        public decimal Amount { get; set; }
        public string TerminalId { get; set; }
        public string BankCode { get; set; }
        public string Message { get; set; }
        public DateTime PayDate { get; set; }
    }
}
