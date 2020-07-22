using System;
using System.Threading.Tasks;
using BlazorApp.Server.Managers;
using BlazorApp.Server.Services;
using Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace BlazorApp.Server.Hubs
{
    public class BattleshipHub : Hub
    {
        private readonly IGamePlayRelay _gamePlayRelay;
        private readonly PlayerManager _playerManager;

        public BattleshipHub(
            IGamePlayRelay gamePlayRelay,
            PlayerManager playerManager)
        {
            _gamePlayRelay = gamePlayRelay;
            _playerManager = playerManager;
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

            if (_playerManager.PlayerCount > 1)
            {
                await base.Clients.All.SendAsync("GameModeChanged", GameMode.Exit);
                _gamePlayRelay.ResetGame();
            }
        }
    }
}