using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Models;
using Core.Models.Ships;

namespace BlazorApp.Server.Services
{
    public interface ISetupRelay
    {
        bool IsPlayerSlotAvailable();
        bool IsOtherPlayerCreated();
        Task<Player> CreatePlayer(
            string name, 
            PlayerType type,
            bool playVsComputer, 
            string connectionId);
        Task PlayerIsReady(
            Guid playerId, 
            List<Ship> ships);
    }
}