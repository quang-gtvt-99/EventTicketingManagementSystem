using EventTicketingMananagementSystem.Core.Models;

namespace EventTicketingMananagementSystem.Core.Dtos
{
    public class ChangePasswordDto
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
        public required string ConfirmedNewPassword { get; set; }
        public required User User { get; set; }
    }
}
