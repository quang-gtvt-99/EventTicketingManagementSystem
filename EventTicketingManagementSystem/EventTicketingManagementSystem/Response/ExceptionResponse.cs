namespace EventTicketingManagementSystem.Response
{
    public class ExceptionResponse
    {
        public string Title { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string? StackTrace { get; set; }
        public string? InnerExceptionMessage { get; set; }
    }
}
