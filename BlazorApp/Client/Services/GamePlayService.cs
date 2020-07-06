using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Ships;
using Shared;
using static Core.Models.CoordinatesHelper;

namespace BlazorApp.Client.Services
{
    public interface IGamePlayService
    {
        Task<Player> CreatePlayerAsync(string name, PlayerType type);
        Task<ICollection<Ship>> GetShipsAsync();
        Task<ICollection<CoordinateContainer>> GetCoordinatesAsync();
        Task MarkCoordinateAsync(Column column, int row);
        Task PlayerReadyAsync(List<Ship> ships);
    }

    // Should handle gameplay related stuff, such as mark coordinate, get ships, get coordinates
    public class GamePlayService : IGamePlayService
    {
        private readonly HttpClient _httpClient;
        private readonly IMessageService _messageService;

        public GamePlayService(HttpClient httpClient, IMessageService messageService)
        {
            _httpClient = httpClient;
            _messageService = messageService;
        }

        public async Task<ICollection<Ship>> GetShipsAsync()
        {
            return null;
        }

        public async Task<ICollection<CoordinateContainer>> GetCoordinatesAsync()
        {
            return null;
        }

        public async Task MarkCoordinateAsync(Column column, int row)
        {

        }

        public async Task PlayerReadyAsync(List<Ship> ships)
        {
            // Do http call to 'setup/ready/{playerId}' which is a Guid..

            var response = await _httpClient.PostAsJsonAsync(
                "setup/ready/b43a3a53-eacf-4b05-9984-8d8fbdfa0cd6",
               ships);

            response.EnsureSuccessStatusCode();
        }

        public async Task<Player> CreatePlayerAsync(string name, PlayerType type)
        {
            // if(!_messageService.IsConnected)
            //     return null;

            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"setup/createplayer/{_messageService.HubConnectionId}",
                    new CreatePlayerDto(type, name));

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<Player>();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to create player: " + exception.Message);
            }

            return null;
        }
    }
}
