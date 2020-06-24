using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Models.Ships;
using static Core.Models.CoordinatesHelper;

namespace Core.Utilities
{
    public static class ShipValidator
    {
        public static void ValidateCoordinates<T>(this T ship, List<(Column column, int row)> coordinates)
            where T : IShip
        {
            var length = coordinates.Count;
            if (coordinates.Any(coord => coord.row > GameConstants.MaxRowCount))
            {
                throw new CoordinatesHelper.CoordinateException("One or more of the coordinates are outside bounds.");
            }

            if (length != ship.Boxes)
            {
                throw new ShipValidationException(
                    $"Ship of type {ship.Name} must have {ship.Boxes} coordinates! {length} coordinates were passed.");
            }

            ValidateCoordinatesAreInLine<T>(ship, coordinates);
        }

        private static void ValidateCoordinatesAreInLine<T>(this T ship, List<(Column column, int row)> coordinates)
            where T : IShip
        {
            var isVertical = coordinates.Select(coord => coord.column).Distinct().Count() == 1;
            var isSequential = IsSequential(coordinates.Select(coord => (int)coord.column).ToArray());

            if (isVertical && isSequential)
                return;

            if (!isSequential)
                throw new ShipValidationException($"Ship {ship.Name} does not have coordinates marked in sequence. Check the coordinates again!");

            var isHorizontal = coordinates.Select(coord => coord.row).Distinct().Count() == 1;
            isSequential = IsSequential(coordinates.Select(coord => coord.row).ToArray());

            if (isHorizontal && isHorizontal)
                return;

            if (!isSequential)
                throw new ShipValidationException($"Ship {ship.Name} does not have coordinates marked in sequence. Check the coordinates again!");

            throw new ShipValidationException($"Ship {ship.Name} is not vertical or horizontal. Check the coordinates again!");
        }

        private static bool IsSequential(int[] coords)
            => Enumerable.Range(1, coords.Length - 1).All(index => coords[index] - 1 == coords[index - 1]);

    }

    public class ShipValidationException : Exception
    {
        public ShipValidationException(string message) : base(message)
        {

        }
    }
}

