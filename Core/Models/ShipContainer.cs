using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models
{
    public class ShipContainer
    {
        public List<CoordinateContainer> Coordinates { get; } = new List<CoordinateContainer>();

        public bool IsDestroyed => Coordinates.All(coord => coord.IsMarked);

        public ShipType ShipType { get; }

        public int Boxes => ShipType.NrOfBoxes();

        private ShipValidator _shipValidator;

        public ShipContainer(ShipType shipType)
        {
            ShipType = shipType;
            _shipValidator = new ShipValidator(ShipType);
        }

        public ShipContainer WithCoordinates(IEnumerable<(Column column, int row)> coordinates)
        {
            SetCoordinates(coordinates);
            return this;
        }

        public bool HasCoordinate(string key) => Coordinates.Any(coord => coord.Key == key);

        public void MarkCoordinate(string key) => Coordinates.Single(coord => coord.Key == key).WasMarked();

        public void SetCoordinates(IEnumerable<(Column column, int row)> coordinates)
        {
            _shipValidator.ValidateCoordinates(coordinates.ToList());

            foreach (var coord in coordinates)
            {
                Coordinates.Add(new CoordinateContainer(coord.column, coord.row).WithShip());
            }
        }
    }
}

