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
        Task LoadOpponentGameBoardAsync();
        Task MarkCoordinateAsync(Column column, int row);
        Guid PlayerId { get; }
        bool IsPlayerSlotAvailable { get; }
    }

    public class GamePlayService : IGamePlayService
    {
        private readonly HttpClient _httpClient;
        private readonly IMessageService _messageService;
        private readonly IEventExecutor _eventExecutor;

        public bool IsPlayerSlotAvailable { get; private set; } = true;

        public Guid PlayerId { get; private set; }

        public GamePlayService(
            HttpClient httpClient,
            IMessageService messageService,
            IEventExecutor eventExecutor)
        {
            _httpClient = httpClient;
            _messageService = messageService;
            _eventExecutor = eventExecutor;
        }

        public async Task PreLoadPlayerSlotAvailable() => IsPlayerSlotAvailable = await IsPlayerSlotAvailableAsync();

        public async Task MarkCoordinateAsync(Column column, int row)
        {
            var _ = await PostRequest<ShipMarkedDto, MarkCoordinateDto>(
                $"gameplay/markcoordinate/{PlayerId}",
                new MarkCoordinateDto(column, row));
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
            var gameBoard = await GetRequest<GameBoardBase>($"gameplay/gameboard/{PlayerId}");
            _eventExecutor.GameBoardChanged(gameBoard);
        }

        public async Task LoadOpponentGameBoardAsync()
        {
            var gameBoard = await GetRequest<GameBoardBase>($"gameplay/opponentgameboard/{PlayerId}");
            _eventExecutor.OpponentGameBoardChanged(gameBoard);
        }

        public Task<bool> IsPlayerSlotAvailableAsync() => GetRequest<bool>("setup/IsPlayerSlotAvailable");

        public Task<bool> IsOtherPlayerCreated() => GetRequest<bool>("setup/IsOtherPlayerCreated");

        public async Task CreatePlayerAsync(string name, PlayerType type, bool playVsComputer)
        {
            try
            {
                var player = await PostRequest<Player, CreatePlayerDto>(
                    $"setup/createplayer/{_messageService.HubConnectionId}",
                    new CreatePlayerDto(type, name, playVsComputer));

                PlayerId = player.Id;
                _eventExecutor.PlayerCreated(player);
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

        private async Task<T> PostRequest<T, TValue>(string url, TValue body)
        {
            var response = await _httpClient.PostAsJsonAsync<TValue>(
               url,
               body);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
