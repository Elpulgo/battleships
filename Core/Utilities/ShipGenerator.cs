using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Models;
using Core.Models.Ships;
using static Core.Models.CoordinatesHelper;

namespace Core.Utilities
{
    public class ShipGenerator
    {
        private const int MaxTriesPerShip = 100;
        private readonly Random _random;
        public ShipGenerator()
        {
            _random = new Random();
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
            var coordinates = new List<(Column, int)>();

            var tries = 0;

            while (coordinates.Count != shipType.NrOfBoxes() && tries < MaxTriesPerShip)
            {
                tries++;
                coordinates.Clear();

                var startingRow = _random.Next(1, GameConstants.MaxRowCount - shipType.NrOfBoxes());
                var startingColumn = _random.Next(1, GameConstants.MaxColumnCount - shipType.NrOfBoxes());

                if (takenCoordinates.ContainsKey(CoordinateKey.Build(startingColumn, startingRow)))
                    continue;

                var direction = (Direction)_random.Next(2);

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

            if (coordinates.Count == shipType.NrOfBoxes())
                return new Ship(shipType, coordinates);

            return GenerateShipInASweepingFashion(ref takenCoordinates, shipType);
        }

        // Failed to generate by randomness in 100 tries, generate first available instead by sweeping the board, to avoid infinite loop
        private Ship GenerateShipInASweepingFashion(
            ref Dictionary<string, string> takenCoordinates,
            ShipType shipType)
        {
            return (Direction)_random.Next(2) switch
            {
                Direction.Horizontal => CreateHorizontalShip(ref takenCoordinates),
                Direction.Vertical => CreateVerticalShip(ref takenCoordinates),
                _ => throw new Exception("Must provide a direction to create ships in a sweeping fashion!")
            };

            Ship CreateHorizontalShip(ref Dictionary<string, string> takenCoordinates)
            {
                foreach (var col in Enumerable.Range(1, GameConstants.MaxColumnCount))
                {
                    // Is outside bounds
                    if (col + shipType.NrOfBoxes() > GameConstants.MaxColumnCount)
                        continue;

                    foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
                    {
                        var proposedKeys = new List<string>();
                        var firstKey = CoordinateKey.Build(col, row);
                        if (takenCoordinates.ContainsKey(firstKey))
                            continue;

                        proposedKeys.Add(firstKey);

                        // Increment by nr of boxes, to check if all boxes from current coord and forward in horizontal
                        // direction is available for taking
                        foreach (var box in Enumerable.Range(1, shipType.NrOfBoxes() - 1))
                        {
                            var key = CoordinateKey.Build((Column)col + box, row);

                            if (takenCoordinates.ContainsKey(key))
                                break;

                            proposedKeys.Add(key);
                        }

                        if (proposedKeys.Count == shipType.NrOfBoxes() &&
                            EnsureCoordinatesAreNotTaken(ref takenCoordinates, proposedKeys, out var coords))
                        {
                            return new Ship(shipType, coords);
                        }
                    }
                }

                throw new Exception("Failed to generate horizontal ship after looking through whole board..");
            }

            Ship CreateVerticalShip(ref Dictionary<string, string> takenCoordinates)
            {
                foreach (var col in Enumerable.Range(1, GameConstants.MaxColumnCount))
                {
                    foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
                    {
                        // Is outside bounds
                        if (row + shipType.NrOfBoxes() > GameConstants.MaxRowCount)
                            continue;

                        var proposedKeys = new List<string>();
                        var firstKey = CoordinateKey.Build(col, row);
                        if (takenCoordinates.ContainsKey(firstKey))
                            continue;

                        proposedKeys.Add(firstKey);

                        // Increment by nr of boxes, to check if all boxes from current coord and forward in vertical
                        // direction is available for taking
                        foreach (var box in Enumerable.Range(1, shipType.NrOfBoxes() - 1))
                        {
                            var key = CoordinateKey.Build((Column)col, row + box);

                            if (takenCoordinates.ContainsKey(key))
                                break;

                            proposedKeys.Add(key);
                        }

                        if (proposedKeys.Count == shipType.NrOfBoxes() &&
                            EnsureCoordinatesAreNotTaken(ref takenCoordinates, proposedKeys, out var coords))
                        {
                            return new Ship(shipType, coords);
                        }
                    }
                }

                throw new Exception("Failed to generate vertical ship after looking through whole board..");
            }
        }

        private bool EnsureCoordinatesAreNotTaken(
            ref Dictionary<string, string> takenCoordinates,
            List<string> proposedKeys,
            out List<(Column Column, int Row)> coordinates)
        {
            coordinates = new List<(Column Column, int Row)>();

            foreach (var key in proposedKeys)
            {
                if (takenCoordinates.ContainsKey(key))
                    return false;
            }

            foreach (var key in proposedKeys)
            {
                takenCoordinates.Add(key, key);
                coordinates.Add(CoordinateKey.Parse(key));
            }

            return true;
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

