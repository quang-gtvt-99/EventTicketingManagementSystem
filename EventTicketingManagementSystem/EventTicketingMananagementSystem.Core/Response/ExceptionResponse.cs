namespace EventTicketingManagementSystem.API.Response
{
    public class ExceptionResponse
    {
        public string Title { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? StackTrace { get; set; }
        public string? InnerExceptionMessage { get; set; }
    }
}
