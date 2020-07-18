using Microsoft.AspNetCore.SignalR;

namespace BlazorApp.Server.Hubs
{
    public class BattleshipHub : Hub
    {

        public BattleshipHub()
        {
        }

        public string GetConnectionId() => Context.ConnectionId;
    }
}