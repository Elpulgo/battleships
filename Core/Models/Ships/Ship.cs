using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models.Ships
{
    [Serializable]
    public class Ship
    {
        public Ship()
        {
        }

        public Ship(ShipType shipType, IEnumerable<(Column column, int row)> coordinates)
        {
            ShipType = shipType;
            ShipValidator.ValidateCoordinates<Ship>(this, coordinates.ToList());

            Coordinates = BuildCoordinates(coordinates).ToList();
        }

        public ICollection<CoordinateContainerBase> Coordinates { get; set; }
        public bool IsDestroyed => Coordinates.All(coord => coord.IsMarked);

        public int Boxes => ShipType.NrOfBoxes();

        public string Name => ShipType.ToString();

        public Color Color => ShipType.GetColor();

        public ShipType ShipType { get; }

        public bool HasCoordinate(string key) => Coordinates.Any(coord => coord.Key == key);

        public void MarkCoordinate(string key) => Coordinates.Single(coord => coord.Key == key).Mark();

        private IEnumerable<CoordinateContainerBase> BuildCoordinates(IEnumerable<(Column column, int row)> coordinates)
        {
            foreach (var coord in coordinates)
            {
                yield return new CoordinateContainerBase(coord.column, coord.row).WithShip().WithColor(Color);
            }
        }
    }
}

