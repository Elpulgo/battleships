using System;
using System.Threading.Tasks;
using Core.Managers;
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
        private readonly ConnectionManager _connectionManager;
        private readonly PlayerManager _playerManager;
        private readonly IPushNotificationService _pushNotificationService;

        public GamePlayController(
            IGameManager gameManager,
            ConnectionManager connectionManager,
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

            await _pushNotificationService.ReloadOpponentGameBoardAsync(playerId);

            var opponentPlayer = _playerManager.GetOpponent(playerId);
            await _pushNotificationService.ReloadGameBoardAsync(opponentPlayer.Id, new ShipMarkedDto(shipFound, shipDestroyed));
            await _pushNotificationService.PlayerTurnChangedAsync(opponentPlayer.Id);

            return Ok(new ShipMarkedDto(shipFound, shipDestroyed));
        }

        [HttpGet("gameboard/{playerid}")]
        public IActionResult GetGameBoard(Guid playerId) => Ok(_gameManager.GetGameBoard(playerId));

        [HttpGet("opponentgameboard/{playerid}")]
        public IActionResult GetOpponentGameBoard(Guid playerId) => Ok(_gameManager.GetOpponentBoard(playerId));
    }
}