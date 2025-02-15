using EventTicketingManagementSystem.Dtos;

namespace EventTicketingManagementSystem.Response
{
    public class EventListResponse
    {
        public IEnumerable<EventInfoDto> Events { get; set; } = default!;
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
