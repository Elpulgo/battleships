using System;
using System.Collections.Generic;
using Core.Models;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal class MonteCarloPrediciton
    {
        private readonly Random _random;

        public MonteCarloPrediciton()
        {
            _random = new Random();
        }

        public (Column Column, int Row, Action<bool, bool> markFromResult) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {

            var resultFromMarkAction = new Action<bool, bool>(WasHit);

            Column column = Column.A;
            int row = 1;

            var availableCoordinate = false;

            while (!availableCoordinate)
            {
                var randomColumn = _random.Next(1, 10);
                var randomRow = _random.Next(1, 10);

                var key = CoordinateKey.Build((Column)randomColumn, randomRow);
                if (!currentGameBoardState[key].IsMarked)
                {
                    column = (Column)randomColumn;
                    row = randomRow;
                    availableCoordinate = true;
                }
            }

            return (column, row, resultFromMarkAction);
        }

        private void WasHit(bool shipFound, bool shipDestroyed)
        {

        }
    }
}