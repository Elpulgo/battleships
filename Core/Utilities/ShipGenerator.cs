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


        // Direction. TODO: Make this better
        // 0 = Horizontal
        // 1 = Vertical

        private IEnumerable<ShipContainer> GenerateShips()
        {
            var random = new Random();

            var shipContainers = new List<ShipContainer>();
            var takenCoodinateKeys = new List<string>();

            foreach (var ship in ShipConstants.GetShipTypesPerPlayer())
            {
                var shipContainer = new ShipContainer(ship);

                var coordinates = new List<CoordinateContainer>();
                var innerCoordinateKeys = new List<string>();
                int tries = 0;
                while (coordinates.Count != ship.NrOfBoxes())
                {
                    tries++;

                    coordinates.Clear();
                    innerCoordinateKeys.Clear();
                    var orientation = random.Next(2);
                    // System.Console.WriteLine("Random: " + orientation);
                    if (orientation == 0)
                    {
                        // System.Console.WriteLine("orientation == 0 (horizontal)");

                        var startingRow = random.Next(1, CoordinatesHelper.GetRowCount() - ship.NrOfBoxes());
                        var startingColumn = random.Next(1, CoordinatesHelper.GetColumnCount() - ship.NrOfBoxes());
                        var firstCoord = new CoordinateContainer((CoordinatesHelper.Column)startingColumn, startingRow);

                        if (takenCoodinateKeys.Any(coord => coord == $"{(CoordinatesHelper.Column)startingColumn}{startingRow}"))
                        {
                            // System.Console.WriteLine("Will break");

                            continue;
                        }

                        var coordinateTaken = false;

                        for (var index = startingColumn; index < startingColumn + ship.NrOfBoxes(); index++)
                        {
                            if (coordinateTaken)
                            {
                                continue;
                            }

                            var newCoord = new CoordinateContainer((CoordinatesHelper.Column)index, startingRow);

                            if (takenCoodinateKeys.Any(coord => coord == $"{(CoordinatesHelper.Column)index}{startingRow}"))
                            {
                                // System.Console.WriteLine("Will break inner loop");
                                coordinateTaken = true;
                                continue;
                            }

                            // System.Console.WriteLine("Wont break");

                            coordinates.Add(newCoord);
                            innerCoordinateKeys.Add($"{(CoordinatesHelper.Column)index}{startingRow}");
                        }
                    }
                    else
                    {
                        // System.Console.WriteLine("orientation == 1 (vertical)");

                        var startingRow = random.Next(1, CoordinatesHelper.GetRowCount() - ship.NrOfBoxes());
                        var startingColumn = random.Next(1, CoordinatesHelper.GetColumnCount() - ship.NrOfBoxes());
                        var firstCoord = new CoordinateContainer((CoordinatesHelper.Column)startingColumn, startingRow);

                        if (takenCoodinateKeys.Any(coord => coord == $"{(CoordinatesHelper.Column)startingColumn}{startingRow}"))
                        {
                            // System.Console.WriteLine("Will break");
                            continue;
                        }

                        var coordinateTaken = false;
                        for (var index = startingRow; index < startingRow + ship.NrOfBoxes(); index++)
                        {
                            if (coordinateTaken)
                            {
                                continue;
                            }

                            var newCoord = new CoordinateContainer((CoordinatesHelper.Column)startingColumn, index);

                            if (takenCoodinateKeys.Any(coord => coord == $"{(CoordinatesHelper.Column)startingColumn}{index}"))
                            {
                                // System.Console.WriteLine("Will break inner loop");
                                coordinateTaken = true;
                                continue;
                            }

                            // System.Console.WriteLine("Wont break");

                            coordinates.Add(newCoord);
                            innerCoordinateKeys.Add($"{(CoordinatesHelper.Column)startingColumn}{index}");
                        }
                    }
                }

                System.Console.WriteLine($"{tries} tries for shiptype {ship.ToString()}");
                takenCoodinateKeys.AddRange(innerCoordinateKeys);
                shipContainer.SetCoordinates(coordinates.Select(s => (s.Column, s.Row)));
                shipContainers.Add(shipContainer);
            }

            return shipContainers;
        }
    }
}

