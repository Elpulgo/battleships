using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class ShipContainer
    {
        public List<CoordinateContainer> Coordinates { get; } = new List<CoordinateContainer>();

        public bool IsDestroyed => Coordinates.All(coord => coord.IsHit);

        public ShipType ShipType { get; }

        public int Boxes => ShipType as int;

        public ShipContainer(IEnumerable<(Column column, int row)> coordinates, ShipType shipType)
        {
            ShipType = shipType;
            ValidateCoordinates(coordinates.ToList());

            foreach (var coord in coordinates)
            {
                Coordinates.Add(new CoordinateContainer(coord.column, coord.row).WithShip(shipType));
            }
        }

        private void ValidateCoordinates(List<(Column column, int row)> coordinates)
        {
            var length = coordinates.Count;
            if (coordinates.Any(coord => coord.row > CoordinatesHelper.GetRowCount()))
            {
                throw new CoordinatesHelper.CoordinateException("One or more of the coordinates are outside bounds.");
            }

            if (length != shipType as int)
            {
                throw new ShipValidationException(
                    $"Ship of type {this.ShipType.ToString()} must have {this.ShipType as int} coordinates! Only {length} coordinates were passed.");
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

