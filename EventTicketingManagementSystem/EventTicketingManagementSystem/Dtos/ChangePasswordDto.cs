using EventTicketingManagementSystem.Models;

namespace EventTicketingManagementSystem.Dtos
{
    public class ChangePasswordDto
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmedNewPassword { get; set; }
        public string? OneTimePassword { get; set; }
        public required User User { get; set; }
    }
}
