using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Core.Models.Ships;
using Shared;
using BlazorApp.Server.Services;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SetupController : ControllerBase
    {
        private readonly ISetupRelay _setupRelay;

        public SetupController(ISetupRelay setupRelay)
        {
            _setupRelay = setupRelay;
        }

        [HttpGet("isplayerslotavailable")]
        public bool IsPlayerSlotAvailable() => _setupRelay.IsPlayerSlotAvailable();

        [HttpGet("isotherplayercreated")]
        public bool IsOtherPlayerCreated() => _setupRelay.IsOtherPlayerCreated();

        [HttpPost("createplayer/{connectionId}")]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto dto, string connectionId)
        {
            try
            {
                var player = await _setupRelay.CreatePlayer(dto.Name, dto.Type, connectionId, dto.PlayVsComputer, dto.ComputerLevel);
                return Ok(player);
            }
            catch (SetupException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Ready/{playerId}")]
        public async Task<IActionResult> PlayerReady([FromBody] List<Ship> ships, Guid playerId)
        {
            await _setupRelay.PlayerIsReady(playerId, ships);
            return Ok();
        }
    }
}
