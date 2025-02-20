namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class EventBookingInfoDto
    {
        public EventInfoDto EventInfo { get; set; } = new EventInfoDto();
        public List<SeatInfoDto> SeatInfos { get; set; } = new List<SeatInfoDto>();

    }
}
