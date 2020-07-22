using System;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace BlazorApp.Client.Services
{
    public interface IMessageService
    {
        Task InitializeAsync();
        string HubConnectionId { get; }
    }

    public class MessageService : IMessageService, IDisposable
    {
        private const string HUBNAME = "/battleshiphub";
        private readonly HubConnection _hubConnection;
        private readonly IEventExecutor _eventExecutor;

        public string HubConnectionId => _hubConnection.ConnectionId;

        public MessageService(
            NavigationManager navigationManager,
            IEventExecutor eventExecutor)
        {
            _eventExecutor = eventExecutor;
            _hubConnection = new HubConnectionBuilder()
                       .WithUrl(navigationManager.ToAbsoluteUri(HUBNAME))
                       .Build();
        }

        public async Task InitializeAsync()
        {
            _hubConnection.On<GameMode>("GameModeChanged", (gameMode) => _eventExecutor.GameModeChanged(gameMode));

            _hubConnection.On<ShipMarkedDto>("ReloadGameBoard", (result) =>
            {
                _eventExecutor.OpponentMoveFired(result.ShipFound, result.ShipDestroyed);
                _eventExecutor.ReloadGameBoard();
            });

            _hubConnection.On<string>("ReloadOpponentGameBoard", (placeHolder) => _eventExecutor.ReloadOpponentGameBoard());

            _hubConnection.On<bool>("PlayerTurnChanged", (isPlayerTurn) => _eventExecutor.PlayerTurnChanged(isPlayerTurn));
            _hubConnection.On<Guid>("WinnerNominated", (winner) => _eventExecutor.WinnerNominated(winner));

            Console.WriteLine("Initializing hub..");
            await _hubConnection.StartAsync();
        }

        public void Dispose() => _ = _hubConnection.DisposeAsync();
    }
}