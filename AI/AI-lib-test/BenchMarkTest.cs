using AI_lib;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using System.IO;
using System.Threading.Tasks;

namespace AI_lib_test
{
    public class BenchMarkTest
    {
        public BenchMarkTest()
        {
        }

        public GameBoardBase CreateGameBoard()
        {
            var ships = new ShipGenerator().Generate().ToList();
            return new GameBoardBase(new Player("test", PlayerType.Computer)).WithShips(ships);
        }

        [Theory]
        [InlineData(100)]
        public async Task BenchmarkPredictions_Random(int nrOfPredictions)
        {
            var scoreSum = new List<int>();
            var tasks = new List<Task>();

            foreach (var index in Enumerable.Range(1, nrOfPredictions))
            {
                tasks.Add(Task.Run(() =>
                {
                    var result = RunRandomGame();
                    scoreSum.Add(result);
                }));
            }

            await Task.WhenAll(tasks);

            var avg = scoreSum.Average();
            var min = scoreSum.Min();
            var max = scoreSum.Max();

            File.AppendAllText("benchmark-rand.txt", $"\n{min}\t {avg}\t {max}");
        }

        [Theory]
        [InlineData(100)]
        public async Task BenchmarkPredictions_Hunter(int nrOfPredictions)
        {
            var scoreSum = new List<int>();
            var tasks = new List<Task>();

            foreach (var index in Enumerable.Range(1, nrOfPredictions))
            {
                tasks.Add(Task.Run(() =>
                {
                    var result = RunHunterGame();
                    scoreSum.Add(result);
                }));
            }

            await Task.WhenAll(tasks);

            var avg = scoreSum.Average();
            var min = scoreSum.Min();
            var max = scoreSum.Max();

            File.AppendAllText("benchmark-hunter.txt", $"\n{min}\t {avg}\t {max}");
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
