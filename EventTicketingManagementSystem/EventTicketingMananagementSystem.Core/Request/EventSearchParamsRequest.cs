namespace EventTicketingManagementSystem.API.Request
{
    public class EventSearchParamsRequest
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
    }
}
