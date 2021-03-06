
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Server.Hubs;
using BlazorApp.Server.Managers;
using Core.Models;
using Microsoft.AspNetCore.SignalR;
using Shared;

namespace BlazorApp.Server.Services
{
    public interface IPushNotificationService
    {
        Task GameModeChangedAllAsync(GameMode gameMode);
        Task GameModeChangedClientAsync(GameMode gameMode, Guid playerId);
        Task ReloadGameBoardAsync(Guid playerId, bool shipFound, bool shipDestroyed);
        Task ReloadOpponentGameBoardAsync(Guid playerId);
        Task PlayerTurnChangedAsync(Guid playerId);
        Task PlayerWaitChangedAsync(Guid playerId);

        Task GameEndedAllAsync(Guid winningPlayerId);
    }

    public class PushNotificationService : IPushNotificationService
    {
        private readonly IHubContext<BattleshipHub> _hubContext;
        private readonly ConnectionManager _connectionManager;
        private readonly PlayerManager _playerManager;

        public PushNotificationService(
            IHubContext<BattleshipHub> hubContext,
            ConnectionManager connectionManager,
            PlayerManager playerManager)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _playerManager = playerManager;
        }

        public async Task GameModeChangedAllAsync(GameMode gameMode)
            => await _hubContext.SendAll(gameMode, "GameModeChanged");
        public async Task GameModeChangedClientAsync(GameMode gameMode, Guid playerId)
            => await _hubContext.SendClient(GetConnectionId(playerId), gameMode, "GameModeChanged");

        public async Task ReloadGameBoardAsync(Guid playerId, bool shipFound, bool shipDestroyed)
            => await _hubContext.SendClient(GetConnectionId(playerId), new ShipMarkedDto(shipFound, shipDestroyed), "ReloadGameBoard");

        public async Task ReloadOpponentGameBoardAsync(Guid playerId)
            => await _hubContext.SendClient(GetConnectionId(playerId), string.Empty, "ReloadOpponentGameBoard");
        public async Task GameEndedAllAsync(Guid winningPlayerId)
        {
            await _hubContext.SendAll(winningPlayerId, "WinnerNominated");
            await GameModeChangedAllAsync(GameMode.GameEnded);
        }

        public async Task PlayerTurnChangedAsync(Guid playerId)
        {
            var connectionIdPlayerTurn = GetConnectionId(playerId);
            await _hubContext.SendClient(connectionIdPlayerTurn, true, "PlayerTurnChanged");
        }

        public async Task PlayerWaitChangedAsync(Guid playerId)
        {
            var connectionIdPlayerWait = GetConnectionId(playerId);
            await _hubContext.SendClient(connectionIdPlayerWait, false, "PlayerTurnChanged");
        }

        private string GetConnectionId(Guid playerId)
        {
            var connectionId = _connectionManager.GetConnection(playerId);
            if (string.IsNullOrEmpty(connectionId))
                throw new ArgumentNullException($"Connection id was not found for player with id '{playerId}'");

            return connectionId;
        }
    }

    internal static class HubExtensions
    {
        public static Task SendAll(this IHubContext<BattleshipHub> hub, object param, string methodName)
            => hub.Clients.All.SendAsync(methodName, param);

        public static Task SendClient(this IHubContext<BattleshipHub> hub, string connectionId, object param, string methodName)
            => hub.Clients.Client(connectionId).SendAsync(methodName, param);
    }
}