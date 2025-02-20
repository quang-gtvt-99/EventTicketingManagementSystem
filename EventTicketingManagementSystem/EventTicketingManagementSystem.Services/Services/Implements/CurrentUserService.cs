using System.Diagnostics.CodeAnalysis;
using EventTicketingManagementSystem.Services.Services.Interfaces;

namespace EventTicketingManagementSystem.Services.Services.Implements
{
    [ExcludeFromCodeCoverage]
    public class CurrentUserService : ICurrentUserService
    {
        public string? Email { get; set; }
        public string? Id { get; set; }
        public string? FullName { get; set; }
    }
}
