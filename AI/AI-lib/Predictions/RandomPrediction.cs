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
            Column column = Column.A;
            int row = 1;

            var availableCoords = BuildFlattenedKeys().ToList();
            availableCoords.Shuffle();

            
            var availableCoordinate = false;

            while (!availableCoordinate && availableCoords.Count > 0)
            {
                var index = _random.Next(0, availableCoords.Count - 1);
                var randomKey = availableCoords[index];

                if (!currentGameBoardState[randomKey].IsMarked)
                {
                    var coord = CoordinateKey.Parse(randomKey);
                    column = coord.Column;
                    row = coord.Row;
                    availableCoordinate = true;
                    break;
                }

                availableCoords.Remove(randomKey);
            }

            return (column, row);
        }



        private Action<MarkCoordinateCallback> GetEmptyCallback()
            => new Action<MarkCoordinateCallback>((_) => { });

        private IEnumerable<string> BuildFlattenedKeys()
        {
            foreach (var col in Enumerable.Range(1, GameConstants.MaxColumnCount))
            {
                foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
                {
                    yield return CoordinateKey.Build((Column)col, row);
                }
            }
        }
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