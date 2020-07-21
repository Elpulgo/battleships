using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace BlazorApp.Server.Hubs
{
    public class BattleshipHub : Hub
    {

        public BattleshipHub()
        {
        }

        public string GetConnectionId() => Context.ConnectionId;

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"User with id {Context.ConnectionId} connected..");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"User with id {Context.ConnectionId} disconnected..");
            await base.OnDisconnectedAsync(exception);
        }
    }
}