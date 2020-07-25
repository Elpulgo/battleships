using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal static class NeighbourCalculator
    {
        public static Neighbours GetNeighbours(List<string> lastFiveHits, Direction direction, Range range)
            => (direction, range) switch
            {
                (Direction.Horizontal, Range.Min) => GetMinHorizontal(lastFiveHits),
                (Direction.Horizontal, Range.Max) => GetMaxHorizontal(lastFiveHits),
                (Direction.Vertical, Range.Min) => GetMinVertical(lastFiveHits),
                (Direction.Vertical, Range.Max) => GetMaxHorizontal(lastFiveHits),
                _ => throw new ArgumentException("This combination is not valid for fetching neighbours!")
            };

        public static bool AreHits(Direction direction, List<string> coordKeys)
            => direction switch
            {
                Direction.Horizontal => AreHitsHorizontal(coordKeys),
                Direction.Vertical => AreHitsVeritcal(coordKeys),
                _ => throw new ArgumentException("This direction is not valid for checking direction of coords!")
            };

        private static bool AreHitsVeritcal(List<string> coordKeys)
        {
            var parsedCoords = coordKeys.Select(key => CoordinateKey.Parse(key)).ToList();
            return parsedCoords.GroupBy(g => g.Column).Count() == 1;
        }

        private static bool AreHitsHorizontal(List<string> coordKeys)
        {
            var parsedCoords = coordKeys.Select(key => CoordinateKey.Parse(key)).ToList();
            return parsedCoords.GroupBy(g => g.Row).Count() == 1;
        }

        private static Neighbours GetMinHorizontal(List<string> lastFiveHits)
        {
            var hitMinHorizontalCoord = lastFiveHits
                .Select(s =>
                {
                    var coord = CoordinateKey.Parse(s);
                    return ((int)coord.Column, coord.Row);
                })
                .OrderBy(m => m.Item1)
                .First();

            var key = CoordinateKey.Build((Column)hitMinHorizontalCoord.Item1, hitMinHorizontalCoord.Row);
            return CoordinateNeighbours.Instance.GetNeighbours(key);
        }

        private static Neighbours GetMaxHorizontal(List<string> lastFiveHits)
        {
            var hitMaxHorizontalCoord = lastFiveHits
                .Select(s =>
                {
                    var coord = CoordinateKey.Parse(s);
                    return ((int)coord.Column, coord.Row);
                })
                .OrderBy(m => m.Item1)
                .Last();

            var key = CoordinateKey.Build((Column)hitMaxHorizontalCoord.Item1, hitMaxHorizontalCoord.Row);
            return CoordinateNeighbours.Instance.GetNeighbours(key);
        }

        private static Neighbours GetMinVertical(List<string> lastFiveHits)
        {
            var hitMinVerticalCoord = lastFiveHits
                .Select(s =>
                {
                    var coord = CoordinateKey.Parse(s);
                    return ((int)coord.Column, coord.Row);
                })
                .OrderBy(m => m.Row)
                .First();

            var key = CoordinateKey.Build((Column)hitMinVerticalCoord.Item1, hitMinVerticalCoord.Row);
            return CoordinateNeighbours.Instance.GetNeighbours(key);
        }

        private static Neighbours GetMaxVertical(List<string> lastFiveHits)
        {
            var hitMaxVerticalCoord = lastFiveHits
                .Select(s =>
                {
                    var coord = CoordinateKey.Parse(s);
                    return ((int)coord.Column, coord.Row);
                })
                .OrderBy(m => m.Row)
                .First();

            var key = CoordinateKey.Build((Column)hitMaxVerticalCoord.Item1, hitMaxVerticalCoord.Row);
            return CoordinateNeighbours.Instance.GetNeighbours(key);
        }
    }

    internal enum Range
    {
        Min,
        Max
    }

    internal enum Direction
    {
        Horizontal,
        Vertical
    }
}