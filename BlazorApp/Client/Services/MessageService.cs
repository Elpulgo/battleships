using System;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace BlazorApp.Client.Services
{


    public interface IMessageService
    {

    }

    // Should handle messages and SignalR related stuff
    public class MessageService : IMessageService, IDisposable
    {
        private readonly HubConnection _hubConnection;
        private readonly EventService _eventService;

        public MessageService(
            NavigationManager navigationManager,
            EventService eventService)
        {
            _eventService = eventService;
            _hubConnection = new HubConnectionBuilder()
                       .WithUrl(navigationManager.ToAbsoluteUri("/battleshiphub"))
                       .Build();

            new Action(async () => await Initialize()).Invoke();
        }



        // Task Send() =>
        //         hubConnection.SendAsync("SendMessage", userInput, messageInput); *@

        //     public bool IsConnected =>
        //         hubConnection.State == HubConnectionState.Connected;

        private async Task Initialize()
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

            await _hubConnection.StartAsync();
        }

        public void Dispose()
        {
            _ = _hubConnection.DisposeAsync();
        }
    }
}