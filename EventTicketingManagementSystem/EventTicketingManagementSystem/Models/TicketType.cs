namespace EventTicketingManagementSystem.Models
{
    public class TicketType
    {
        public int TicketTypeId { get; set; }
        public int EventId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int TotalQuantity { get; set; }
        public int RemainingQuantity { get; set; }
        public string Description { get; set; }
        public int MaxPerCustomer { get; set; }
        public DateTime SaleStartDate { get; set; }
        public DateTime SaleEndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; } = "Active";

        // Navigation properties
        public Event Event { get; set; }
        public ICollection<Ticket> Tickets { get; set; }
        public ICollection<BookingDetail> BookingDetails { get; set; }
    }
}
