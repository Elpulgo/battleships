using System;
using System.Collections.Generic;
using System.Linq;
using static Core.Models.CoordinatesHelper;

namespace Core.Models
{
    public class ShipContainer
    {
        public List<CoordinateContainer> Coordinates { get; } = new List<CoordinateContainer>();

        public bool IsDestroyed => Coordinates.All(coord => coord.IsMarked);

        public ShipType ShipType { get; }

        public int Boxes => ShipType.NrOfBoxes();

        public ShipContainer(ShipType shipType) => ShipType = shipType;

        public bool HasCoordinate(string key) => Coordinates.Any(coord => coord.Key == key);

        public void MarkCoordinate(string key) => Coordinates.Single(coord => coord.Key == key).WasMarked();

        public void SetCoordinates(IEnumerable<(Column column, int row)> coordinates)
        {
            ValidateCoordinates(coordinates.ToList());

            foreach (var coord in coordinates)
            {
                Coordinates.Add(new CoordinateContainer(coord.column, coord.row).WithShip());
            }
        }

        private void ValidateCoordinates(List<(Column column, int row)> coordinates)
        {
            var length = coordinates.Count;
            if (coordinates.Any(coord => coord.row > CoordinatesHelper.GetRowCount()))
            {
                throw new CoordinatesHelper.CoordinateException("One or more of the coordinates are outside bounds.");
            }

            if (length != ShipType.NrOfBoxes())
            {
                throw new ShipValidationException(
                    $"Ship of type {this.ShipType.ToString()} must have {ShipType.NrOfBoxes()} coordinates! Only {length} coordinates were passed.");
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

