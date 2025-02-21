namespace EventTicketingManagementSystem.Response
{
    public class PaymentResponse
    {
        public int BookingId { get; set; }
        public long VnPayTranId { get; set; }
        public string ResponseCode { get; set; } = string.Empty;  
        public string TransactionStatus { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TerminalId { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime PayDate { get; set; }
    }
}
