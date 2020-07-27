using System;
using System.Collections.Generic;
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

            var availableCoordinate = false;

            while (!availableCoordinate)
            {
                var randomColumn = _random.Next(1, GameConstants.MaxColumnCount);
                var randomRow = _random.Next(1, GameConstants.MaxRowCount);

                var key = CoordinateKey.Build((Column)randomColumn, randomRow);
                if (!currentGameBoardState[key].IsMarked)
                {
                    column = (Column)randomColumn;
                    row = randomRow;
                    availableCoordinate = true;
                }
            }

            return (column, row);
        }

        private Action<MarkCoordinateCallback> GetEmptyCallback()
            => new Action<MarkCoordinateCallback>((_) => { });
    }
}