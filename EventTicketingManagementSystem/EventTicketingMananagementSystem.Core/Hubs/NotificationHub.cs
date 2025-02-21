using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace EventTicketingMananagementSystem.Core.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task RegisterUser(string email)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, email);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }

}
