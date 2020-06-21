using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace Core.Utilities
{
    public class ShipValidator
    {
        private ShipType ShipType { get; }
        public ShipValidator(ShipType shipType) => ShipType = shipType;

        public void ValidateCoordinates(List<(Column column, int row)> coordinates)
        {
            var length = coordinates.Count;
            if (coordinates.Any(coord => coord.row > GameConstants.MaxRowCount))
            {
                throw new CoordinatesHelper.CoordinateException("One or more of the coordinates are outside bounds.");
            }

            if (length != ShipType.NrOfBoxes())
            {
                throw new ShipValidationException(
                    $"Ship of type {this.ShipType.ToString()} must have {ShipType.NrOfBoxes()} coordinates! {length} coordinates were passed.");
            }

            ValidateCoordinatesAreInLine(coordinates);
        }

        private void ValidateCoordinatesAreInLine(List<(Column column, int row)> coordinates)
        {

            var isVertical = coordinates.Select(coord => coord.column).Distinct().Count() == 1;

            if (!isVertical)
            {
                var isHorizontal = coordinates.Select(coord => coord.row).Distinct().Count() == 1;
                if (!isHorizontal)
                {
                    throw new ShipValidationException($"Ship {ShipType.ToString()} is not vertical or horizontal. Check the coordinates again!");
                }
            }
        }
    }

    public class ShipValidationException : Exception
    {
        public ShipValidationException(string message) : base(message)
        {

        }
    }
}

