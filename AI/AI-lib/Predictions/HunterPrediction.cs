using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal class HunterPrediction
    {
        private readonly RandomPrediction _randomPrediction;
        private readonly Random _random;
        private List<string> _lastFiveHits;
        private bool IsInHuntMode => _lastFiveHits.Any();
        public HunterPrediction()
        {
            _randomPrediction = new RandomPrediction();
            _random = new Random();
            _lastFiveHits = new List<string>();
        }

        public (Column Column, int Row, Action<MarkCoordinateCallback> resultFromMark) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            var markCallback = new Action<MarkCoordinateCallback>(WasHit);

            if (!IsInHuntMode)
            {
                var mark = _randomPrediction.Predict(currentGameBoardState);
                return (mark.Column, mark.Row, markCallback);
            }

            var (column, row) = PredictNext(currentGameBoardState);

            return (column, row, markCallback);
        }

        private void WasHit(MarkCoordinateCallback callback)
        {
            if (callback.ShipDestroyed)
            {
                foreach (var coord in callback.DestroyedShipCoordinates)
                {
                    _lastFiveHits.Remove(coord);
                }

                return;
            }

            if (!callback.ShipFound)
                return;

            if (_lastFiveHits.Count < 5)
            {
                _lastFiveHits.Add(callback.Key);
                return;
            }

            _lastFiveHits = _lastFiveHits
                                .Skip(1)
                                .Append(callback.Key)
                                .ToList();
        }

        private (Column Column, int Row) PredictNext(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            var horizontal = NeighbourCalculator.AreHits(Direction.Horizontal, _lastFiveHits);
            var vertical = NeighbourCalculator.AreHits(Direction.Vertical, _lastFiveHits);

            if ((horizontal && vertical) || (!horizontal && !vertical))
                return GetRandomNeighbour(currentGameBoardState);

            if (horizontal && TryPredictHorizontal(currentGameBoardState, out var horizontalCoord))
                return horizontalCoord;

            if (vertical && TryPredictVertical(currentGameBoardState, out var verticalCoord))
                return verticalCoord;

            return GetRandomNeighbour(currentGameBoardState);
        }

        private bool TryPredictHorizontal(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState,
            out (Column Column, int Row) predictedCoord)
        {
            var minNeighbours = NeighbourCalculator.GetNeighbours(_lastFiveHits, Direction.Horizontal, Range.Min);
            var maxNeighbours = NeighbourCalculator.GetNeighbours(_lastFiveHits, Direction.Horizontal, Range.Max);

            var isMinLeftOpenForMove =
                !string.IsNullOrEmpty(minNeighbours.NeighbourLeft) &&
                !currentGameBoardState[minNeighbours.NeighbourLeft].IsMarked;

            var isMaxRightOpenForMove =
                !string.IsNullOrEmpty(maxNeighbours.NeighbourRight) &&
                !currentGameBoardState[maxNeighbours.NeighbourRight].IsMarked;

            if (isMinLeftOpenForMove && isMaxRightOpenForMove)
            {
                var random = _random.Next(1);
                if (random > 0)
                {
                    predictedCoord = CoordinateKey.Parse(minNeighbours.NeighbourLeft);
                    return true;
                }
                else
                {
                    predictedCoord = CoordinateKey.Parse(maxNeighbours.NeighbourRight);
                    return true;
                }
            }

            if (isMinLeftOpenForMove)
            {
                predictedCoord = CoordinateKey.Parse(minNeighbours.NeighbourLeft);
                return true;
            }

            if (isMaxRightOpenForMove)
            {
                predictedCoord = CoordinateKey.Parse(maxNeighbours.NeighbourRight);
                return true;
            }

            predictedCoord = (Column.A, 0);
            return false;
        }

        private bool TryPredictVertical(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState,
            out (Column Column, int Row) predictedCoord)
        {
            var minNeighbours = NeighbourCalculator.GetNeighbours(_lastFiveHits, Direction.Vertical, Range.Min);
            var maxNeighbours = NeighbourCalculator.GetNeighbours(_lastFiveHits, Direction.Vertical, Range.Max);

            var isMinUpOpenForMove =
                !string.IsNullOrEmpty(minNeighbours.NeighbourUp) &&
                !currentGameBoardState[minNeighbours.NeighbourUp].IsMarked;

            var isMaxDownOpenForMove =
                !string.IsNullOrEmpty(maxNeighbours.NeighbourDown) &&
                !currentGameBoardState[maxNeighbours.NeighbourDown].IsMarked;

            if (isMinUpOpenForMove && isMaxDownOpenForMove)
            {
                var random = _random.Next(1);
                if (random > 0)
                {
                    predictedCoord = CoordinateKey.Parse(minNeighbours.NeighbourUp);
                    return true;
                }
                else
                {
                    predictedCoord = CoordinateKey.Parse(maxNeighbours.NeighbourDown);
                    return true;
                }
            }

            if (isMinUpOpenForMove)
            {
                predictedCoord = CoordinateKey.Parse(minNeighbours.NeighbourUp);
                return true;
            }

            if (isMaxDownOpenForMove)
            {
                predictedCoord = CoordinateKey.Parse(maxNeighbours.NeighbourDown);
                return true;
            }

            predictedCoord = (Column.A, 0);
            return false;
        }

        private (Column Column, int Row) GetRandomNeighbour(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            var lastHitNeighboursNotMarked = _lastFiveHits.Select(s => s).ToList();

            // Take random neighbour for last hits and check if neighbours are already marked
            while (lastHitNeighboursNotMarked.Any())
            {
                var random = _random.Next(lastHitNeighboursNotMarked.Count - 1);
                var hitCoordKey = lastHitNeighboursNotMarked[random];
                var neighbours = CoordinateNeighbours.Instance.GetNeighbours(hitCoordKey);

                // Take random neighbour x times and return if not already marked
                var neighbourCount = neighbours.AvailableNeighbours.Count;
                while (neighbourCount > 0)
                {
                    var randomNeighbourIndex = _random.Next(neighbours.AvailableNeighbours.Count - 1);
                    var randomNeighbour = neighbours.AvailableNeighbours[randomNeighbourIndex];
                    if (!currentGameBoardState[randomNeighbour].IsMarked)
                    {
                        lastHitNeighboursNotMarked.Clear();
                        return (CoordinateKey.Parse(randomNeighbour));
                    }

                    neighbourCount--;
                }

                if (lastHitNeighboursNotMarked.Any())
                {
                    lastHitNeighboursNotMarked.Remove(hitCoordKey);
                }
            }

            // If all neighbours for the last hits are marked, fallback to random prediction
            return _randomPrediction.PredictWithoutCallback(currentGameBoardState);
        }
    }
}