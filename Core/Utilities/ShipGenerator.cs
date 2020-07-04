using System;
using System.Collections.Generic;
using Core.Models;
using Core.Models.Ships;
using static Core.Models.CoordinatesHelper;

namespace Core.Utilities
{
    public class ShipGenerator
    {
        public ShipGenerator()
        {
        }

        public IEnumerable<Ship> Generate() => GenerateShips();

        private IEnumerable<Ship> GenerateShips()
        {
            var takenCoodinateKeys = new Dictionary<string, string>();

            foreach (var ship in ShipConstants.GetShipTypesPerPlayer())
            {
                yield return GenerateShipFor(ship, ref takenCoodinateKeys);
            }
        }

        private Ship GenerateShipFor(
            ShipType shipType,
            ref Dictionary<string, string> takenCoordinates)
        {
            var random = new Random();

            var coordinates = new List<(Column, int)>();

            while (coordinates.Count != shipType.NrOfBoxes())
            {
                coordinates.Clear();

                var startingRow = random.Next(1, GameConstants.MaxRowCount - shipType.NrOfBoxes());
                var startingColumn = random.Next(1, GameConstants.MaxColumnCount - shipType.NrOfBoxes());

                if (takenCoordinates.ContainsKey(CoordinateKey.Build(startingColumn, startingRow)))
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

                for (var index = startingCount; index < startingCount + shipType.NrOfBoxes(); index++)
                {
                    var column = direction == Direction.Horizontal ? index : startingColumn;
                    var row = direction == Direction.Vertical ? index : startingRow;

                    if (!TryAddCoordinateForBox(column, row, ref takenCoordinates))
                        continue;

                    coordinates.Add(((CoordinatesHelper.Column)column, row));
                }
            }

            return new Ship(shipType, coordinates);
        }

        private bool TryAddCoordinateForBox(
            int column,
            int row,
            ref Dictionary<string, string> takenCoordinates)
        {
            var key = CoordinateKey.Build(column, row);

            if (takenCoordinates.TryAdd(key, key))
                return true;

            return false;
        }
    }

    enum Direction
    {
        Horizontal = 0,
        Vertical = 1
    }
}

