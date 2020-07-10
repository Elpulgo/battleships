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
        Task CreatePlayerAsync(string name, PlayerType type, bool playVsComputer);
        Task<bool> IsPlayerSlotAvailableAsync();
        Task<bool> IsOtherPlayerCreated();
        Task PreLoadPlayerSlotAvailable();
        Task PlayerReadyAsync(List<Ship> ships);
        Task LoadGameBoardAsync();

        Task<ICollection<Ship>> GetShipsAsync();
        Task<ICollection<CoordinateContainer>> GetCoordinatesAsync();
        Task MarkCoordinateAsync(Column column, int row);


        Guid PlayerId { get; }
        bool IsPlayerSlotAvailable { get; }
    }

    // Should handle gameplay related stuff, such as mark coordinate, get ships, get coordinates
    public class GamePlayService : IGamePlayService
    {
        private readonly HttpClient _httpClient;
        private readonly IMessageService _messageService;
        private readonly IEventService _eventService;

        public bool IsPlayerSlotAvailable { get; private set; } = true;

        public Guid PlayerId { get; private set; }

        public GamePlayService(
            HttpClient httpClient,
            IMessageService messageService,
            IEventService eventService)
        {
            _httpClient = httpClient;
            _messageService = messageService;
            _eventService = eventService;
        }

        public async Task PreLoadPlayerSlotAvailable() => IsPlayerSlotAvailable = await IsPlayerSlotAvailableAsync();

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
            var response = await _httpClient.PostAsJsonAsync(
                $"setup/ready/{PlayerId}",
                ships);

            response.EnsureSuccessStatusCode();
        }

        public async Task LoadGameBoardAsync()
        {
            var gameBoard = await GetRequest<GameBoard>($"gameplay/gameboard/{PlayerId}");
            _eventService.GameBoardChanged(gameBoard);
        }

        public Task<bool> IsPlayerSlotAvailableAsync() => GetRequest<bool>("setup/IsPlayerSlotAvailable");

        public Task<bool> IsOtherPlayerCreated() => GetRequest<bool>("setup/IsOtherPlayerCreated");

        public async Task CreatePlayerAsync(string name, PlayerType type, bool playVsComputer)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    $"setup/createplayer/{_messageService.HubConnectionId}",
                    new CreatePlayerDto(type, name, playVsComputer));

                response.EnsureSuccessStatusCode();

                var player = await response.Content.ReadFromJsonAsync<Player>();
                PlayerId = player.Id;
                _eventService.PlayerCreated(player);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to create player: " + exception.Message);
            }
        }

        private async Task<T> GetRequest<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
