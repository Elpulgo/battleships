using Microsoft.AspNetCore.Mvc;
using BlazorApp.Server.Hubs;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Models.Ships;
using Shared;
using Microsoft.AspNetCore.SignalR;
using BlazorApp.Server.Managers;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SetupController : ControllerBase
    {
        private readonly IHubContext<BattleshipHub> _hubContext;
        private readonly ConnectionManager<Player> _connectionManager;
        private readonly PlayerManager _playerManager;

        public SetupController(
            IHubContext<BattleshipHub> hubContext,
            ConnectionManager<Player> connectionManager,
            PlayerManager playerManager)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
            _playerManager = playerManager;
        }

        [HttpGet("isplayerslotavailable")]
        public bool IsPlayerSlotAvailable() => (!_playerManager.IsPlayingVsComputer && _connectionManager.Count < 2);

        [HttpGet("isotherplayercreated")]
        public bool IsOtherPlayerCreated() => _playerManager.PlayerCount > 0 && !_playerManager.IsPlayingVsComputer;

        [HttpPost("createplayer/{connectionId}")]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto dto, string connectionId)
        {
            if (_connectionManager.Count > 1)
                return BadRequest("Maximum number of players already in the game!");

            var player = new Player(dto.Name, dto.Type);
            _connectionManager.Add(player, connectionId);
            _playerManager.AddPlayerToGame(player);

            if (dto.PlayVsComputer)
            {
                _playerManager.PlayVsComputer();
            }

            return Ok(player);
        }

        [HttpPost("Ready/{playerId}")]
        public async Task<IActionResult> PlayerReady([FromBody] List<Ship> request, Guid playerId)
        {
            var apa = Core.Models.GameMode.WaitingForPlayer;

            Console.WriteLine("Connections is: " + _connectionManager.Count);
            // _hubContext.Clients.

            Console.WriteLine("GM IS: " + apa);
            // await _battleShipHub.SendMessage("aaa", "sss");
            // await _battleShipHub.GameModeChanged(apa);
            // System.Console.WriteLine(request.FirstOrDefault().Name);
            // Body with list of ships? and player
            // Return succes or false, depending on validation?
            return Ok();
        }
    }
}
