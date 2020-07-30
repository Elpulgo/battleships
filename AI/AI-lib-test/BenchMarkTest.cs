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

        [Theory(Skip = "Should only run in debug mode since benchmark test")]
        // [Theory]
        [InlineData(10)]
        public void BenchmarkPredictions_Random(int nrOfPredictions)
        {
            File.Delete("benchmark-random-index.txt");

            var scoreSum = new List<int>();

            foreach (var index in Enumerable.Range(1, nrOfPredictions))
            {
                var result = base.RunRandomGame();
                scoreSum.Add(result);
                var avg_ = scoreSum.Average();
                var min_ = scoreSum.Min();
                var max_ = scoreSum.Max();
                File.AppendAllText("benchmark-random-index.txt", $"\n{index} \t {min_}\t{avg_}\t{max_}");
            }
        }

        [Theory(Skip = "Should only run in debug mode since benchmark test")]
        // [Theory]
        [InlineData(500)]
        public void BenchmarkPredictions_Hunter(int nrOfPredictions)
        {
            File.Delete("benchmark-hunter-index.txt");

            var scoreSum = new List<int>();
            var tasks = new List<Task>();

            foreach (var index in Enumerable.Range(1, nrOfPredictions))
            {
                var result = base.RunHunterGame();
                scoreSum.Add(result);
                var avg_ = scoreSum.Average();
                var min_ = scoreSum.Min();
                var max_ = scoreSum.Max();
                File.AppendAllText("benchmark-hunter-index.txt", $"\n{index} \t {min_}\t{avg_}\t{max_}");
            }
        }

        [Theory(Skip = "Should only run in debug mode since benchmark test")]
        // [Theory]
        [InlineData(10)]
        public void BenchmarkPredictions_MonteCarlo(int nrOfPredictions)
        {
            File.Delete("benchmark-montecarlo-index.txt");

            var scoreSum = new List<int>();
            var tasks = new List<Task>();

            foreach (var index in Enumerable.Range(1, nrOfPredictions))
            {
                var result = base.RunMonteCarloGame();
                scoreSum.Add(result);

                var avg_ = scoreSum.Average();
                var min_ = scoreSum.Min();
                var max_ = scoreSum.Max();
                File.AppendAllText("benchmark-montecarlo-index.txt", $"\n{index} \t {min_}\t{avg_}\t{max_}");
            }
        }

        [Theory(Skip = "Should only run in debug mode since benchmark test")]
        // [Theory]
        [InlineData(10)]
        public void BenchmarkPredictions_MonteCarloWithHunt(int nrOfPredictions)
        {
            File.Delete("benchmark-montecarlowithhunt-index.txt");

            var scoreSum = new List<int>();
            var tasks = new List<Task>();

            foreach (var index in Enumerable.Range(1, nrOfPredictions))
            {
                var result = base.RunMonteCarloWithHuntGame();
                scoreSum.Add(result);

                var avg_ = scoreSum.Average();
                var min_ = scoreSum.Min();
                var max_ = scoreSum.Max();
                File.AppendAllText("benchmark-montecarlowithhunt-index.txt", $"\n{index} \t {min_}\t{avg_}\t{max_}");
            }
        }

        [Fact(Skip = "Should only run in debug mode since benchmark test")]
        public void BenchMarkShipGenerator()
        {
            File.Delete("generate.txt");
            var stopWatch = new Stopwatch();

            for (int i = 0; i < 10; i++)
            {
                stopWatch.Start();
                var ships = new ShipGenerator().Generate().ToList();
                stopWatch.Stop();

                File.AppendAllText("generate.txt", $"\n{i} \t {stopWatch.ElapsedMilliseconds} ms");
            }
        }

        [Fact(Skip = "Should only run in debug mode since benchmark test")]
        public void Benchmark_AverageElapsedTime_PerPrediction_ForRandom()
        {
            var gameBoard = base.CreateGameBoard();
            var elapsed = new List<long>();

            var AIManager = new AIManager();
            var maxScore = 100;
            var stopWatch = new Stopwatch();
            foreach (var move in Enumerable.Range(0, maxScore))
            {
                stopWatch.Restart();
                var (column, row, action) = AIManager.PredictCoordinate(AILevel.Random, gameBoard.ForOpponent().Matrix);
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

        [Fact(Skip = "Should only run in debug mode since benchmark test")]
        public void Benchmark_AverageElapsedTime_PerPrediction_ForHunter()
        {
            var gameBoard = base.CreateGameBoard();
            var elapsed = new List<long>();

            var AIManager = new AIManager();
            var maxScore = 100;
            var stopWatch = new Stopwatch();
            foreach (var move in Enumerable.Range(0, maxScore))
            {
                stopWatch.Restart();
                var (column, row, action) = AIManager.PredictCoordinate(AILevel.Hunter, gameBoard.ForOpponent().Matrix);
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
