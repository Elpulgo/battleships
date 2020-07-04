using Microsoft.AspNetCore.Mvc;
using BlazorApp.Server.Hubs;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Models.Ships;
using Shared;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SetupController : ControllerBase
    {
        private readonly BattleshipHub _battleShipHub;

        public SetupController(BattleshipHub battleShipHub)
        {
            _battleShipHub = battleShipHub;
        }

        [HttpPost]
        public IActionResult AddPlayer()
        {
            // Body with name and gamemode(human or computer)
            // Return Core.Model.Player with id as Guid, connected to signalR guid id?
            // Exception/Error model if 2 players already exist
            return null;

        }

        [HttpPost("Ready/{playerId}")]
        public async Task<IActionResult> PlayerReady([FromBody] List<ShipBase> request, Guid playerId)
        {
            await _battleShipHub.GameModeChanged(Core.Models.GameMode.WaitingForPlayer);
            System.Console.WriteLine(request.FirstOrDefault().Name);
            // Body with list of ships? and player
            // Return succes or false, depending on validation?
            return Ok();
        }
    }
}
