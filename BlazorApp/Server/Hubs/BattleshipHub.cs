using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Core.Models;

namespace BlazorApp.Server.Hubs
{
    public class BattleshipHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task GameModeChanged(GameMode gameMode) => await Clients.All.SendAsync("GameModeChanged", gameMode);
    }
}