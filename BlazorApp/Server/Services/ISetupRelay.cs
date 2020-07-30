using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorApp.Shared;
using Core.Models;
using Core.Models.Ships;
using Shared;

namespace BlazorApp.Server.Services
{
    public interface ISetupRelay
    {
        bool IsPlayerSlotAvailable();
        bool IsOtherPlayerCreated();
        Task<Player> CreatePlayer(
            string name,
            PlayerType type,
            string connectionId,
            bool playVsComputer = false,
            ComputerLevel computerLevel = ComputerLevel.None);
        Task PlayerIsReady(
            Guid playerId,
            List<Ship> ships);
    }
}