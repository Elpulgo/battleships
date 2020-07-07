
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

    }

    internal static class HubExtensions
    {
        public static Task SendAll(this IHubContext<BattleshipHub> hub, object param, string methodName)
            => hub.Clients.All.SendAsync(methodName, param);

        public static Task SendClient(this IHubContext<BattleshipHub> hub, string connectionId, object param, string methodName)
            => hub.Clients.All.SendAsync(methodName, param);
    }
}