using AI_lib;
using Xunit;
using System.Linq;
using Core.Models;
using Core.Utilities;
using System.Collections.Generic;

namespace AI_lib_test
{
    public class PredictionTest
    {
        public GameBoardBase GameBoard { get; set; }
        public PredictionTest()
        {
            GameBoard = CreateGameBoard();
        }

        public GameBoardBase CreateGameBoard()
        {
            var ships = new ShipGenerator().Generate().ToList();
            return new GameBoardBase(new Player("test", PlayerType.Computer)).WithShips(ships);
        }

        [Fact]
        public void Should_FinishRandomGameWithin_MaxBoxCount()
        {
            var numberOfMoves = RunRandomGame();
            Assert.True(numberOfMoves > 1);
        }

        [Fact]
        public void Should_FinishHunterGameWithin_MaxBoxCount()
        {
            var numberOfMoves = RunHunterGame();
            Assert.True(numberOfMoves > 1);
        }

        private int RunRandomGame()
        {
            var gameBoard = CreateGameBoard();

            var maxCount = 100;
            var AIManager = new AIManager(AILevel.Random);

            foreach (var move in Enumerable.Range(1, maxCount))
            {
                var (column, row, action) = AIManager.PredictCoordinate(gameBoard.ForOpponent().Matrix);
                var (_, _x) = gameBoard.MarkCoordinate(CoordinateKey.Build(column, row));

                maxCount--;
                if (gameBoard.IsAllDestroyed())
                {
                    break;
                }
            }

            return maxCount;
        }

        private int RunHunterGame()
        {
            var gameBoard = CreateGameBoard();

            var maxScore = 100;
            var AIManager = new AIManager(AILevel.Hunter);

            foreach (var move in Enumerable.Range(1, maxScore))
            {
                var (column, row, action) = AIManager.PredictCoordinate(gameBoard.ForOpponent().Matrix);
                var key = CoordinateKey.Build(column, row);
                var (shipFound, shipDestroyed) = gameBoard.MarkCoordinate(key);

                var callback = new MarkCoordinateCallback(shipFound, key);

                if (shipDestroyed)
                {
                    var coordsForShip = gameBoard.GetCoordinatesForDestroyedShip(key);
                    callback = callback.WithDestroyedShip(coordsForShip);
                }
                action.Invoke(callback);

                maxScore--;
                if (gameBoard.IsAllDestroyed())
                {
                    break;
                }
            }

            return maxScore;
        }
    }
}
