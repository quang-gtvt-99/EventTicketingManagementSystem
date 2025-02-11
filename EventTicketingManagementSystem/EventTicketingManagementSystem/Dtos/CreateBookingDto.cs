namespace EventTicketingManagementSystem.Dtos
{
    public class CreateBookingDto
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public List<CreateSeatDto> SeatedInfos { get; set; } = new List<CreateSeatDto>();
    }
}
