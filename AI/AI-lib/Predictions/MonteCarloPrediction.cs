using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Models;
using Core.Models.Ships;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal class MonteCarloPrediciton : HunterPrediction
    {
        private static readonly Lazy<MonteCarloPrediciton> lazy
        = new Lazy<MonteCarloPrediciton>(() => new MonteCarloPrediciton());

        public static new MonteCarloPrediciton Instance { get { return lazy.Value; } }

        private const int NumberOfSimulations = 20;
        private readonly ShipGenerator _shipGenerator;

        protected MonteCarloPrediciton()
        {
            _shipGenerator = new ShipGenerator();
        }

        public override (Column Column, int Row, Action<MarkCoordinateCallback> callback) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            base.BuildHits(currentGameBoardState);
            var markCallback = new Action<MarkCoordinateCallback>(base.WasHit);

            var prediction = PredictBySimulation(currentGameBoardState);
            return (prediction.Column, prediction.Row, markCallback);
        }

        protected (Column Column, int Row) PredictBySimulation(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            var orderedByHighestProbability = SimulateShipsOnBoard();

            if (FindNonMarkedBoxFromHighestProbability(
                orderedByHighestProbability,
                currentGameBoardState,
                out var foundKey))
            {
                return CoordinateKey.Parse(foundKey);
            }

            // If no available box has been found after 20 simulations, fallback to random prediction
            return base.PredictRandom(currentGameBoardState);
        }

        private List<KeyValuePair<string, int>> SimulateShipsOnBoard()
        {
            var markedCoordGroup = new Dictionary<string, int>();
            var ships = new List<Ship>();

            foreach (var index in Enumerable.Range(1, NumberOfSimulations))
            {
                ships.AddRange(_shipGenerator.Generate().ToList());
            }

            var keys = ships
                .SelectMany(s => s.Coordinates)
                .Select(s => s.Key)
                .ToList();

            foreach (var key in keys)
            {
                if (markedCoordGroup.TryGetValue(key, out var count))
                {
                    markedCoordGroup[key] = ++count;
                    continue;
                }

                markedCoordGroup.TryAdd(key, 1);
            }

            return markedCoordGroup
                .OrderByDescending(o => o.Value)
                .ToList();
        }

        private bool FindNonMarkedBoxFromHighestProbability(
            List<KeyValuePair<string, int>> probabilities,
            Dictionary<string, CoordinateContainerBase> currentGameBoardState,
            out string foundKey)
        {
            foundKey = string.Empty;

            foreach (var probability in probabilities)
            {
                if (currentGameBoardState[probability.Key].IsMarked)
                    continue;

                foundKey = probability.Key;
                return true;
            }

            return false;
        }
    }
}