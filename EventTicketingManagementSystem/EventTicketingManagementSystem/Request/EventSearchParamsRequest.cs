namespace EventTicketingManagementSystem.Request
{
    public class EventSearchParamsRequest
    {
        public string? Search { get; set; }
        public string? Category { get; set; }
        public string? Status { get; set; }
        public int PageNumber { get; set; } = 1;  // Default to 1 if not provided
        public int PageSize { get; set; } = 5;  // Default to 5 if not provided
    }
}
