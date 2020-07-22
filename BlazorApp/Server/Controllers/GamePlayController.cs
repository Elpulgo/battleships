using System;
using System.Threading.Tasks;
using Shared;
using Microsoft.AspNetCore.Mvc;
using BlazorApp.Server.Services;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamePlayController : ControllerBase
    {
        private readonly IGamePlayRelay _gamePlayRelay;

        public GamePlayController(IGamePlayRelay gamePlayRelay)
        {
            _gamePlayRelay = gamePlayRelay;
        }

        [HttpPost("markcoordinate/{playerId}")]
        public async Task<IActionResult> MarkCoordinate([FromBody] MarkCoordinateDto request, Guid playerId)
        {
            var (shipFound, shipDestroyed) = await _gamePlayRelay.MarkCoordinateAsync(
                request.Column,
                request.Row,
                playerId
            );
            return Ok(new ShipMarkedDto(shipFound, shipDestroyed));
        }

        [HttpGet("gameboard/{playerid}")]
        public IActionResult GetGameBoard(Guid playerId)
            => Ok(_gamePlayRelay.GetGameBoard(playerId));

        [HttpGet("opponentgameboard/{playerid}")]
        public IActionResult GetOpponentGameBoard(Guid playerId)
            => Ok(_gamePlayRelay.GetOpponentGameBoard(playerId));

        [HttpGet("finalboards/{playerId}")]
        public IActionResult GetFinalBoards(Guid playerId)
        {
            var (board, opponentBoard) = _gamePlayRelay.GetFinalBoards(playerId);
            _gamePlayRelay.ResetGame();
            return Ok(new FinalBoardsDto(board, opponentBoard));
        }
    }
}