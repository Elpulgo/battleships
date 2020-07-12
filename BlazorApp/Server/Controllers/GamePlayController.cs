using System;
using System.Threading.Tasks;
using Core.Managers;
using Core.Models;
using Shared;
using Microsoft.AspNetCore.Mvc;
using BlazorApp.Server.Services;
using Core.Utilities;
using BlazorApp.Server.Managers;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamePlayController : ControllerBase
    {
        private readonly IGameManager _gameManager;
        private readonly ConnectionManager<Player> _connectionManager;
        private readonly PlayerManager _playerManager;
        private readonly IPushNotificationService _pushNotificationService;

        public GamePlayController(
            IGameManager gameManager,
            ConnectionManager<Player> connectionManager,
            PlayerManager playerManager,
            IPushNotificationService pushNotificationService)
        {
            _gameManager = gameManager;
            _connectionManager = connectionManager;
            _playerManager = playerManager;
            _pushNotificationService = pushNotificationService;
        }

        [HttpPost("markcoordinate/{playerId}")]
        public async Task<IActionResult> MarkCoordinate([FromBody] MarkCoordinateDto request, Guid playerId)
        {
            var (shipFound, shipDestroyed) = _gameManager.MarkCoordinate(
                playerId,
                CoordinateKey.Build(request.Column, request.Row));

            await ReloadGameBoard(playerId);
            await ReloadOpponentBoard(playerId);

            return Ok(new ShipMarkedDto(shipFound, shipDestroyed));

            async Task ReloadGameBoard(Guid playerId)
            {
                var player = _playerManager.GetPlayerById(playerId);
                var connectionId = _connectionManager.GetConnection(player);
                await _pushNotificationService.ReloadOpponentGameBoardAsync(connectionId);
            }

            async Task ReloadOpponentBoard(Guid playerId)
            {
                var opponentPlayer = _playerManager.GetOpponent(playerId);
                var connectionId = _connectionManager.GetConnection(opponentPlayer);
                await _pushNotificationService.ReloadGameBoardAsync(connectionId);
            }
        }

        [HttpGet("gameboard/{playerid}")]
        public IActionResult GetGameBoard(Guid playerId) => Ok(_gameManager.GetGameBoard(playerId));

        [HttpGet("opponentgameboard/{playerid}")]
        public IActionResult GetOpponentGameBoard(Guid playerId) => Ok(_gameManager.GetOpponentBoard(playerId));
    }
}