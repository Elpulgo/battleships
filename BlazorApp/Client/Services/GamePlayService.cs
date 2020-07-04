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
        Task<ICollection<IShip>> GetShipsAsync();
        Task<ICollection<CoordinateContainer>> GetCoordinatesAsync();
        Task MarkCoordinateAsync(Column column, int row);
        Task PlayerReadyAsync(List<IShip> ships);
    }

    // Should handle gameplay related stuff, such as mark coordinate, get ships, get coordinates
    public class GamePlayService : IGamePlayService
    {
        private readonly HttpClient _httpClient;

        public GamePlayService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ICollection<IShip>> GetShipsAsync()
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

        public async Task PlayerReadyAsync(List<IShip> ships)
        {
            // Do http call to 'setup/ready/{playerId}' which is a Guid..

            var response = await _httpClient.PostAsJsonAsync(
                "setup/ready/b43a3a53-eacf-4b05-9984-8d8fbdfa0cd6",
               ships);

            response.EnsureSuccessStatusCode();
            System.Console.WriteLine("In PlayerReadyAsync..");
        }
    }
}
