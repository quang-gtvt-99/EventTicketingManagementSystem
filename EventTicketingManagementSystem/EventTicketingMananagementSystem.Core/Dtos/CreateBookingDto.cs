namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class CreateBookingDto
    {
        //public int UserId { get; set; }
        public int EventId { get; set; }
        public List<UpdateSeatDto> SeatedInfos { get; set; } = new List<UpdateSeatDto>();
    }
}
