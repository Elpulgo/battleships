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

        public SetupController(IHubContext<BattleshipHub> hubContext, ConnectionManager<Player> connectionManager)
        {
            _hubContext = hubContext;
            _connectionManager = connectionManager;
        }

        [HttpPost("createplayer/{connectionId}")]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto dto, string connectionId)
        {
            if (_connectionManager.Count > 1)
                return BadRequest("Maximum number of players already in the game!");

            var player = new Player(dto.Name, dto.Type);
            _connectionManager.Add(player, connectionId);

            Console.WriteLine("Connectionid is: " + connectionId);
            Console.WriteLine("Connections is: " + _connectionManager.Count);
            Console.WriteLine("Will activate 'GameModeChanged' now!");


            await _hubContext.Clients.All.SendAsync("GameModeChanged", GameMode.WaitingForPlayer);
            // Body with name and gamemode(human or computer)
            // Return Core.Model.Player with id as Guid, connected to signalR guid id?
            // Exception/Error model if 2 players already exist
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
