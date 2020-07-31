using AI_lib;
using System.Linq;
using Core.Models;
using Core.Utilities;

namespace AI_lib_test
{
    public class GameRunnerHelper
    {
        public GameRunnerHelper()
        {
        }

        public GameBoardBase CreateGameBoard()
        {
            var ships = new ShipGenerator().Generate().ToList();
            return new GameBoardBase(new Player("test", PlayerType.Computer)).WithShips(ships);
        }

        public int RunRandomGame()
        {
            var gameBoard = CreateGameBoard();

            var maxScore = 100;
            var AIManager = new AIManager();

            foreach (var move in Enumerable.Range(0, maxScore))
            {
                var (column, row) = AIManager.PredictCoordinate(AILevel.Random, gameBoard.ForOpponent().Matrix);
                var (_, _x) = gameBoard.MarkCoordinate(CoordinateKey.Build(column, row));

                maxScore--;
                if (gameBoard.IsAllDestroyed())
                {
                    return maxScore;
                }
            }

            return maxScore;
        }

        public int RunHunterGame()
        {
            var gameBoard = CreateGameBoard();

            var maxScore = 100;
            var AIManager = new AIManager();

            foreach (var move in Enumerable.Range(0, maxScore))
            {
                var (column, row) = AIManager.PredictCoordinate(AILevel.Hunter, gameBoard.ForOpponent().Matrix);

                var key = CoordinateKey.Build(column, row);
                var (shipFound, shipDestroyed) = gameBoard.MarkCoordinate(key);

                maxScore--;
                if (gameBoard.IsAllDestroyed())
                {
                    return maxScore;
                }
            }

            return maxScore;
        }

        public int RunMonteCarloGame()
        {
            var gameBoard = CreateGameBoard();

            var maxScore = 100;
            var AIManager = new AIManager();

            foreach (var move in Enumerable.Range(0, maxScore))
            {
                var (column, row) = AIManager.PredictCoordinate(AILevel.MonteCarlo, gameBoard.ForOpponent().Matrix);

                var key = CoordinateKey.Build(column, row);
                var (shipFound, shipDestroyed) = gameBoard.MarkCoordinate(key);

                maxScore--;
                if (gameBoard.IsAllDestroyed())
                {
                    return maxScore;
                }
            }

            return maxScore;
        }

        public int RunMonteCarloWithHuntGame()
        {
            var gameBoard = CreateGameBoard();

            var maxScore = 100;
            var AIManager = new AIManager();

            foreach (var move in Enumerable.Range(0, maxScore))
            {
                var (column, row) = AIManager.PredictCoordinate(AILevel.MonteCarloAndHunt, gameBoard.ForOpponent().Matrix);

                var key = CoordinateKey.Build(column, row);
                var (shipFound, shipDestroyed) = gameBoard.MarkCoordinate(key);

                maxScore--;
                if (gameBoard.IsAllDestroyed())
                {
                    return maxScore;
                }
            }

            return maxScore;
        }
    }
}
