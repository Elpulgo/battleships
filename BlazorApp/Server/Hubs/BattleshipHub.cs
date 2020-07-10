using Microsoft.AspNetCore.SignalR;
using Core.Models;
using BlazorApp.Server.Managers;

namespace BlazorApp.Server.Hubs
{
    public class BattleshipHub : Hub
    {
        private readonly ConnectionManager<Player> _connectionManager;

        public BattleshipHub(ConnectionManager<Player> connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public string GetConnectionId() => Context.ConnectionId;
    }
}