namespace EventTicketingManagementSystem.Models
{
    public class BookingDetail
    {
        public int BookingDetailId { get; set; }
        public int BookingId { get; set; }
        public int TicketTypeId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }

        // Navigation properties
        public Booking Booking { get; set; }
        public TicketType TicketType { get; set; }
    }
}
