using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal class RandomPrediction
    {
        private readonly Random _random;

        public RandomPrediction()
        {
            _random = new Random();
        }

        public (Column Column, int Row, Action<MarkCoordinateCallback> callback) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            var prediction = PredictWithoutCallback(currentGameBoardState);
            return (prediction.Column, prediction.Row, GetEmptyCallback());
        }

        public (Column Column, int Row) PredictWithoutCallback(
           Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            var availableCoords = BuildFlattenedKeys(currentGameBoardState);
            availableCoords.Shuffle();

            if (!availableCoords.Any())
                throw new ArgumentOutOfRangeException("Can't predict a coordinate when all coordinates are marked! Game should have ended by now!");

            return CoordinateKey.Parse(availableCoords.First());
        }

        private Action<MarkCoordinateCallback> GetEmptyCallback()
            => new Action<MarkCoordinateCallback>((_) => { });

        private List<string> BuildFlattenedKeys(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
            => currentGameBoardState
                .Where(w => !w.Value.IsMarked)
                .Select(s => s.Key)
                .ToList();

    }

    internal static class Extensions
    {
        private static Random random = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}