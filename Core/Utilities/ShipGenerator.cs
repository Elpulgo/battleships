using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace Core.Utilities
{
    public class ShipGenerator
    {
        public ShipGenerator()
        {

        }

        public IEnumerable<ShipContainer> Generate() => GenerateShips();

        private IEnumerable<ShipContainer> GenerateShips()
        {
            var takenCoodinateKeys = new Dictionary<string, string>();

            foreach (var ship in ShipConstants.GetShipTypesPerPlayer())
            {
                yield return GenerateShipFor(ship, ref takenCoodinateKeys);
            }
        }

        private ShipContainer GenerateShipFor(
            ShipType ship,
            ref Dictionary<string, string> takenCoordinates)
        {
            var random = new Random();

            var coordinates = new List<CoordinateContainer>();

            while (coordinates.Count != ship.NrOfBoxes())
            {
                coordinates.Clear();

                var startingRow = random.Next(1, CoordinatesHelper.GetRowCount() - ship.NrOfBoxes());
                var startingColumn = random.Next(1, CoordinatesHelper.GetColumnCount() - ship.NrOfBoxes());

                if (takenCoordinates.ContainsKey($"{(CoordinatesHelper.Column)startingColumn}{startingRow}"))
                {
                    continue;
                }

                var direction = (Direction)random.Next(2);

                var startingCount = direction switch
                {
                    Direction.Horizontal => startingColumn,
                    Direction.Vertical => startingRow,
                    _ => throw new Exception("Passed ship direction does not exist!")
                };

                for (var index = startingCount; index < startingCount + ship.NrOfBoxes(); index++)
                {
                    var column = direction == Direction.Horizontal ? index : startingColumn;
                    var row = direction == Direction.Vertical ? index : startingRow;

                    if (!TryAddCoordinateForBox(column, row, ref takenCoordinates, out var coord))
                        continue;

                    coordinates.Add(coord);
                }
            }

            return new ShipContainer(ship)
                .WithCoordinates(coordinates.Select(s => (s.Column, s.Row)));
        }

        private bool TryAddCoordinateForBox(
            int column,
            int row,
            ref Dictionary<string, string> takenCoordinates,
            out CoordinateContainer coord)
        {
            coord = null;
            var key = $"{(CoordinatesHelper.Column)column}{row}";

            if (takenCoordinates.TryAdd(key, key))
            {
                coord = new CoordinateContainer((CoordinatesHelper.Column)column, row);
                return true;
            }

            return false;
        }
    }

    enum Direction
    {
        Horizontal = 0,
        Vertical = 1
    }
}

