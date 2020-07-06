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



        // Task Send() =>
        //         hubConnection.SendAsync("SendMessage", userInput, messageInput); *@

        public bool IsConnected => _hubConnection.State == HubConnectionState.Connected;

        public async Task InitializeAsync()
        {
            _hubConnection.On<string, string>("CoordinateMarked", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
            });

            _hubConnection.On<string, string>("GameFinished", (user, message) =>
            {
                var encodedMsg = $"{user}: {message}";
            });


            _hubConnection.On<GameMode>("GameModeChanged", (gameMode) => _eventService.GameModeChanged(gameMode));
            Console.WriteLine("Initializing hub..");
            await _hubConnection.StartAsync();
        }

        public void Dispose() => _ = _hubConnection.DisposeAsync();
    }
}