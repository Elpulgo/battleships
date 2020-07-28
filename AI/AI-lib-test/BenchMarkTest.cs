using AI_lib;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AI_lib_test
{
    public class BenchMarkTest : GameRunnerHelper
    {
        public BenchMarkTest()
        {
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
                    var result = base.RunRandomGame();
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
                    var result = base.RunHunterGame();
                    scoreSum.Add(result);
                }));
            }

            await Task.WhenAll(tasks);

            var avg = scoreSum.Average();
            var min = scoreSum.Min();
            var max = scoreSum.Max();

            File.AppendAllText("benchmark-hunter.txt", $"\n{min}\t {avg}\t {max}");
        }

        [Fact]
        public void Benchmark_AverageElapsedTime_PerPrediction_ForRandom()
        {
            var gameBoard = base.CreateGameBoard();
            var elapsed = new List<long>();

            var AIManager = new AIManager(AILevel.Random);
            var maxScore = 100;
            var stopWatch = new Stopwatch();
            foreach (var move in Enumerable.Range(0, maxScore))
            {
                stopWatch.Restart();
                var (column, row, action) = AIManager.PredictCoordinate(gameBoard.ForOpponent().Matrix);
                stopWatch.Stop();

                elapsed.Add(stopWatch.ElapsedMilliseconds);

                var (_, _x) = gameBoard.MarkCoordinate(CoordinateKey.Build(column, row));

                if (gameBoard.IsAllDestroyed())
                {
                    var min = elapsed.Min();
                    var avg = elapsed.Average();
                    var max = elapsed.Max();

                    File.AppendAllText("benchmark-random-elapsed-time.txt", $"\n{min} ms\t {avg} ms\t {max} ms");
                    return;
                }
            }
        }

        [Fact]
        public void Benchmark_AverageElapsedTime_PerPrediction_ForHunter()
        {
            var gameBoard = base.CreateGameBoard();
            var elapsed = new List<long>();

            var AIManager = new AIManager(AILevel.Hunter);
            var maxScore = 100;
            var stopWatch = new Stopwatch();
            foreach (var move in Enumerable.Range(0, maxScore))
            {
                stopWatch.Restart();
                var (column, row, action) = AIManager.PredictCoordinate(gameBoard.ForOpponent().Matrix);
                stopWatch.Stop();

                elapsed.Add(stopWatch.ElapsedMilliseconds);

                var key = CoordinateKey.Build(column, row);
                var (shipFound, shipDestroyed) = gameBoard.MarkCoordinate(key);

                var callback = new MarkCoordinateCallback(shipFound, key);

                if (shipDestroyed)
                {
                    var coordsForShip = gameBoard.GetCoordinatesForDestroyedShip(key);
                    callback = callback.WithDestroyedShip(coordsForShip);
                }
                action.Invoke(callback);

                if (gameBoard.IsAllDestroyed())
                {
                    var min = elapsed.Min();
                    var avg = elapsed.Average();
                    var max = elapsed.Max();

                    File.AppendAllText("benchmark-hunter-elapsed-time.txt", $"\n{min} ms\t {avg} ms\t {max} ms");
                    return;
                }
            }
        }
    }
}
