namespace EventTicketingManagementSystem.Request
{
    public class UpdateUserProfileRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmedNewPassword { get; set; } = string.Empty;
        public bool AllowChangePassword { get; set; }
    }
}
