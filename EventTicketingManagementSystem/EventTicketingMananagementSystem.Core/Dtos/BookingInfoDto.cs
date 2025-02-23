namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class BookingInfoDto
    {
        public int BookingId { get; set; }
        public int EventId { get; set; }
        public string? EventName { get; set; }
        public string? EventDate { get; set; }
        public string? EventTime { get; set; }
        public string? Venue { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public DateTime BookedAt { get; set; }
        public List<TicketInfoDto> Tickets { get; set; } = new List<TicketInfoDto>();
        public string? QRCode { get; set; }
        public string? BookingCode { get; set; }
    }
}
