namespace EventTicketingManagementSystem.Dtos
{
    public class EventInfoDto
    {
        public int EventID { get; set; }
        public string? EventName { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public string? VenueName { get; set; }
        public string? VenueAddress { get; set; }
        public string? ImageUrls { get; set; }
        public List<SeatInfoDto> SeatInfos { get; set; } = new List<SeatInfoDto>();

    }
}
