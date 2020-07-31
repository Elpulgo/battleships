using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal class HunterPrediction : RandomPrediction
    {
        private static readonly Lazy<HunterPrediction> lazy
           = new Lazy<HunterPrediction>(() => new HunterPrediction());

        public static new HunterPrediction Instance { get { return lazy.Value; } }

        protected readonly CryptoRandomizer _random;
        protected List<string> _hits;
        protected bool IsInHuntMode => _hits.Any();

        protected HunterPrediction()
        {
            _random = new CryptoRandomizer();
            _hits = new List<string>();
        }

        public override (Column Column, int Row) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            BuildHits(currentGameBoardState);

            if (!IsInHuntMode)
                return base.Predict(currentGameBoardState);

            return PredictHunter(currentGameBoardState);
        }

        protected void BuildHits(Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            var hits = currentGameBoardState
                .Where(w => (w.Value.IsMarked && w.Value.HasShip && !w.Value.IsShipDestroyed))
                .Select(s => s.Key)
                .ToList();

            _hits = hits;
        }

        protected (Column Column, int Row) PredictHunter(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            var horizontal = NeighbourCalculator.AreHits(Direction.Horizontal, _hits);
            var vertical = NeighbourCalculator.AreHits(Direction.Vertical, _hits);

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
            var minNeighbours = NeighbourCalculator.GetNeighbours(_hits, Direction.Horizontal, Range.Min);
            var maxNeighbours = NeighbourCalculator.GetNeighbours(_hits, Direction.Horizontal, Range.Max);

            var isMinLeftOpenForMove =
                !string.IsNullOrEmpty(minNeighbours.NeighbourLeft) &&
                !currentGameBoardState[minNeighbours.NeighbourLeft].IsMarked;

            var isMaxRightOpenForMove =
                !string.IsNullOrEmpty(maxNeighbours.NeighbourRight) &&
                !currentGameBoardState[maxNeighbours.NeighbourRight].IsMarked;

            if (isMinLeftOpenForMove && isMaxRightOpenForMove)
            {
                var random = _random.Next(0, 1);
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
            var minNeighbours = NeighbourCalculator.GetNeighbours(_hits, Direction.Vertical, Range.Min);
            var maxNeighbours = NeighbourCalculator.GetNeighbours(_hits, Direction.Vertical, Range.Max);

            var isMinUpOpenForMove =
                !string.IsNullOrEmpty(minNeighbours.NeighbourUp) &&
                !currentGameBoardState[minNeighbours.NeighbourUp].IsMarked;

            var isMaxDownOpenForMove =
                !string.IsNullOrEmpty(maxNeighbours.NeighbourDown) &&
                !currentGameBoardState[maxNeighbours.NeighbourDown].IsMarked;

            if (isMinUpOpenForMove && isMaxDownOpenForMove)
            {
                var random = _random.Next(0, 1);
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
            var lastHitNeighboursNotMarked = _hits.Select(s => s).ToList();

            // Take random neighbour for last hits and check if neighbours are already marked
            while (lastHitNeighboursNotMarked.Any())
            {
                var random = _random.Next(0, lastHitNeighboursNotMarked.Count - 1);
                var hitCoordKey = lastHitNeighboursNotMarked[random];
                var neighbours = CoordinateNeighbours.Instance.GetNeighbours(hitCoordKey);
                var neighbourCount = neighbours.AvailableNeighbours.Count;

                // Take random neighbour until no neighbours left and return if not already marked
                while (neighbourCount > 0)
                {
                    var randomNeighbourIndex = _random.Next(0, neighbourCount - 1);
                    var randomNeighbour = neighbours.AvailableNeighbours[randomNeighbourIndex];
                    if (!currentGameBoardState[randomNeighbour].IsMarked)
                    {
                        return (CoordinateKey.Parse(randomNeighbour));
                    }

                    neighbourCount--;
                }

                lastHitNeighboursNotMarked.Remove(hitCoordKey);
            }

            // If all neighbours for the last hits are marked, fallback to random prediction
            return PredictRandom(currentGameBoardState);
        }

        protected (Column Column, int Row) PredictRandom(Dictionary<string, CoordinateContainerBase> currentGameBoardState)
            => base.Predict(currentGameBoardState);
    }
}