namespace EventTicketingManagementSystem.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public string Category { get; set; }
        public string ArtistInfo { get; set; }
        public string ImageUrls { get; set; }
        public int TotalTickets { get; set; }
        public int RemainingTickets { get; set; }
        public string Status { get; set; } = "Upcoming";
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public User User { get; set; }
        public ICollection<TicketType> TicketTypes { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}
