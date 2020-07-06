using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task GameModeChanged(GameMode gameMode)
        {
            Console.WriteLine("Gamemode is.." + gameMode);

            await Clients.All.SendAsync("GameModeChanged", gameMode);
        }
    }
}