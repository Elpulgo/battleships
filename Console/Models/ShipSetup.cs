using System;
using System.Collections.Generic;
using Console.Print;
using Core.Models.Ships;
using static Core.Models.CoordinatesHelper;

namespace Console.Models
{
    public class ShipSetup
    {
        public ShipType ShipType { get; }

        public List<(Column column, int row)> Coords { get; private set; }

        private List<(int x, int y)> Positions { get; set; }

        public bool IsAllCoordsSet => Coords.Count == ShipType.NrOfBoxes();

        public bool IsLastCoordToSet => Coords.Count == ShipType.NrOfBoxes() - 1;

        public ShipSetup(ShipType shipType)
        {
            ShipType = shipType;
            Positions = new List<(int x, int y)>();
            Coords = new List<(Column column, int row)>();
        }

        public void AddCoord(Column column, int row)
        {
            Coords.Add((column, row));
        }

        public void AddPosition(int x, int y)
        {
            Positions.Add((x, y));
        }

        public void Clear(int currentPosition_X, int currentPosition_Y)
        {
            foreach (var position in Positions)
            {
                System.Console.SetCursorPosition(position.x, position.y);
                " ".Write(Color.None);
            }

            Coords.Clear();
            Positions.Clear();

            System.Console.SetCursorPosition(currentPosition_X, currentPosition_Y);
        }
    }
}
