using EventTicketingManagementSystem.Enums;

namespace EventTicketingManagementSystem.Request
{
    public class AddUpdateEventRequest
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? VenueName { get; set; }
        public string? VenueAddress { get; set; }
        public IFormFile? Image { get; set; }
        public decimal? SeatPrice { get; set; }
        public CategoryEnum? Category { get; set; }
    }
}
