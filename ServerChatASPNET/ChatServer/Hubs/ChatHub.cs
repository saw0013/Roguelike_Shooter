using Microsoft.AspNetCore.SignalR;

namespace ChatServer.Hubs
{
    public class ChatHub : Hub
    {
        public async Task TestSend(string message)
        {
            await Clients.All.SendAsync("Receive", message);
        }
    }
}
