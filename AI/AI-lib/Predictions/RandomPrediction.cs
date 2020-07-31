using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal class RandomPrediction : PredictionBase
    {
        private static readonly Lazy<RandomPrediction> lazy
            = new Lazy<RandomPrediction>(() => new RandomPrediction());

        public static new RandomPrediction Instance { get { return lazy.Value; } }

        protected RandomPrediction()
        {
        }

        public override (Column Column, int Row) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            var availableCoords = BuildFlattenedKeys(currentGameBoardState);
            if (!availableCoords.Any())
                throw new ArgumentOutOfRangeException("Can't predict a coordinate when all coordinates are marked! Game should have ended by now!");

            availableCoords.Shuffle();

            return CoordinateKey.Parse(availableCoords.First());
        }

        private List<string> BuildFlattenedKeys(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
            => currentGameBoardState
                .Where(w => !w.Value.IsMarked)
                .Select(s => s.Key)
                .ToList();

    }
}