using System;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorApp.Client.Services
{
    public interface IMessageService
    {
        Task InitializeAsync();

        string HubConnectionId { get; }

        bool IsConnected { get; }
    }

    public class MessageService : IMessageService, IDisposable
    {
        private const string HUBNAME = "/battleshiphub";
        private readonly HubConnection _hubConnection;
        private readonly IEventService _eventService;

        public string HubConnectionId => _hubConnection.ConnectionId;

        public MessageService(
            NavigationManager navigationManager,
            IEventService eventService)
        {
            _eventService = eventService;
            _hubConnection = new HubConnectionBuilder()
                       .WithUrl(navigationManager.ToAbsoluteUri(HUBNAME))
                       .Build();
        }

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

        public async Task InitializeAsync()
        {
            _hubConnection.On<GameMode>("GameModeChanged", (gameMode) => _eventService.GameModeChanged(gameMode));

            _hubConnection.On<string>("ReloadGameBoard", (placeHolder) => _eventService.ReloadGameBoard());

            _hubConnection.On<string>("ReloadOpponentGameBoard", (placeHolder) => _eventService.ReloadOpponentGameBoard());

            _hubConnection.On<bool>("PlayerTurnChanged", (isPlayerTurn) => _eventService.PlayerTurnChanged(isPlayerTurn));
            
            Console.WriteLine("Initializing hub..");
            await _hubConnection.StartAsync();
        }

        public void Dispose() => _ = _hubConnection.DisposeAsync();
    }
}