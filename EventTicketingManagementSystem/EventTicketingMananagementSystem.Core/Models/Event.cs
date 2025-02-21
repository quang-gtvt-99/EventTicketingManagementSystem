using EventTicketingMananagementSystem.Core.Models.BaseModels;

namespace EventTicketingMananagementSystem.Core.Models
{
    public class Event : EntityAuditBase<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? VenueName { get; set; }
        public string? VenueAddress { get; set; }
        public string? Category { get; set; }
        public string? ArtistInfo { get; set; }
        public string? ImageUrls { get; set; }
        public int? TotalTickets { get; set; }
        public int? RemainingTickets { get; set; }
        public string? Status { get; set; }
        public decimal? SeatPrice { get; set; }
        public int? CreatedBy { get; set; }
        public string? TrailerUrls { get; set; }
        // Navigation properties
        public User User { get; set; } = default!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    }
}
