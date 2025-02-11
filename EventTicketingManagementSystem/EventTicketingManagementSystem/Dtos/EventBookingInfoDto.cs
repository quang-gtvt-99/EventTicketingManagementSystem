namespace EventTicketingManagementSystem.Dtos
{
    public class EventBookingInfoDto
    {
        public EventInfoDto EventInfo { get; set; } 
        public List<SeatInfoDto> SeatInfos { get; set; } = new List<SeatInfoDto>();

    }
}
