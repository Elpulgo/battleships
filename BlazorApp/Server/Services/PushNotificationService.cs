
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Server.Hubs;
using Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace BlazorApp.Server.Services
{
    public interface IPushNotificationService
    {
        Task GameModeChangedAllAsync(GameMode gameMode);
        Task GameModeChangedClientAsync(GameMode gameMode, string connectionId);
        Task ReloadGameBoardAsync(string connectionId);
        Task ReloadOpponentGameBoardAsync(string connectionId);
        Task PlayerTurnChangedAsync(string connectionIdPlayerTurn, string connectionIdPlayerWait);
    }

    public class PushNotificationService : IPushNotificationService
    {
        private readonly IHubContext<BattleshipHub> _hubContext;

        public PushNotificationService(IHubContext<BattleshipHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task GameModeChangedAllAsync(GameMode gameMode)
            => await _hubContext.SendAll(gameMode, "GameModeChanged");
        public async Task GameModeChangedClientAsync(GameMode gameMode, string connectionId)
            => await _hubContext.SendClient(connectionId, gameMode, "GameModeChanged");

        public async Task ReloadGameBoardAsync(string connectionId)
            => await _hubContext.SendClient(connectionId, string.Empty, "ReloadGameBoard");

        public async Task ReloadOpponentGameBoardAsync(string connectionId)
            => await _hubContext.SendClient(connectionId, string.Empty, "ReloadOpponentGameBoard");

        public async Task PlayerTurnChangedAsync(string connectionIdPlayerTurn, string connectionIdPlayerWait)
        {
            await _hubContext.SendClient(connectionIdPlayerTurn, true, "PlayerTurnChanged");
            await _hubContext.SendClient(connectionIdPlayerWait, false, "PlayerTurnChanged");
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