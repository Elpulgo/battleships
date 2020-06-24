using System;
using System.Collections.Generic;
using Core.Models.Ships;
using static Core.Models.CoordinatesHelper;

namespace Console.Models
{
    public class ShipSetup
    {
        public ShipType ShipType { get; }

        public List<(Column, int)> Coords { get; private set; }

        public bool IsAllCoordsSet => Coords.Count == ShipType.NrOfBoxes();

        public bool IsLastCoordToSet => Coords.Count == ShipType.NrOfBoxes() - 1;

        public ShipSetup(ShipType shipType)
        {
            ShipType = shipType;
            Coords = new List<(Column, int)>();
        }

        public void SetCoord(Column column, int row)
        {
            Coords.Add((column, row));
        }

    }
}
