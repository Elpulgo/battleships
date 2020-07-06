using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models.Ships
{
    public class Ship
    {
        public Ship()
        {            
        }
        
        public Ship(ShipType shipType, IEnumerable<(Column column, int row)> coordinates)
        {
            ShipType = shipType;
            ShipValidator.ValidateCoordinates<Ship>(this, coordinates.ToList());
            SetCoordinates(coordinates);
        }

        public ICollection<CoordinateContainer> Coordinates { get; private set; }
        public bool IsDestroyed => Coordinates.All(coord => coord.IsMarked);

        public int Boxes => ShipType.NrOfBoxes();

        public string Name => ShipType.ToString();

        public Color Color => ShipType.GetColor();

        public ShipType ShipType { get; }

        public bool HasCoordinate(string key) => Coordinates.Any(coord => coord.Key == key);

        public void MarkCoordinate(string key) => Coordinates.Single(coord => coord.Key == key).Mark();

        private void SetCoordinates(IEnumerable<(Column column, int row)> coordinates)
        {
            Coordinates = new List<CoordinateContainer>();

            foreach (var coord in coordinates)
            {
                Coordinates.Add(new CoordinateContainer(coord.column, coord.row).WithShip().WithColor(Color));
            }
        }
    }
}

