using System;
using System.Threading.Tasks;
using Core.Managers;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamePlayController : ControllerBase
    {
        private readonly IGameManager _gameManager;

        public GamePlayController(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        [HttpPost]
        public IActionResult MarkCoordinate()
        {
            // Body with player id and coordinate to mark
            // Return hit/miss/ship sunken?
            return null;
        }

        [HttpGet("gameboard/{playerid}")]
        public IActionResult GetGameBoard(Guid playerId) => Ok(_gameManager.GetGameBoard(playerId));

        [HttpGet("opponentgameboard/{playerid}")]
        public IActionResult GetOpponentGameBoard(Guid playerId) => Ok(_gameManager.GetOpponentBoard(playerId));
    }
}