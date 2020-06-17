using System;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using static Core.Models.CoordinatesHelper;
using Core.Utilities;

namespace Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var pro = new Program();
            pro.CreateShipsForPlayer();
        }

        private void CreateShipsForPlayer()
        {
            // var shipContainers = new List<ShipContainer>();
            // foreach (var ship in ShipConstants.GetShipTypesPerPlayer())
            // {
            //     var shipContainer = new ShipContainer(ship);
            //     var coords = GenerateCoordinates(shipContainer.Boxes, 0);
            //     shipContainer.SetCoordinates(coords.ToList());
            //     shipContainers.Add(shipContainer);
            // }

            var shipGenerator = new ShipGenerator();
            var shipContainers = shipGenerator.Generate();

            foreach (var item in shipContainers)
            {
                System.Console.WriteLine($"Shiptype: {item.ShipType}\t IsDestroyed: {item.IsDestroyed}");
                foreach (var coord in item.Coordinates)
                {
                    System.Console.WriteLine($"\tCoord: {coord.Key}\t IsMarked: {coord.IsMarked} \t Has ship: {coord.HasShip}");
                }
            }

        }

        // Direction. TODO: Make this better
        // 0 = Horizontal
        // 1 = Vertical

        private IEnumerable<(Column column, int row)> GenerateCoordinates(int boxes, int direction)
        {
            System.Console.WriteLine("Boxes: " + boxes);
            var coords = new List<(Column, int)>();
            // TODO: Build either horizontally or vertically here to pass the validation..
            var random = new Random();

            foreach (var box in Enumerable.Range(1, boxes))
            {
                var nextRandom = random.Next(1, 10);
                var column = (CoordinatesHelper.Column)nextRandom;
                var coordExist = false;
                while (coords.Any(a => a.Item1 == column && a.Item2 == nextRandom) && coordExist)
                {
                    var newRandom = random.Next(1, 10);
                    var newCoord = (coords.Any(), direction) switch
                    {
                        (true, 0) => ((CoordinatesHelper.Column)newRandom, coords.First().Item2),
                        (true, 1) => (coords.First().Item1, newRandom),
                        (_, _) => (column, nextRandom),
                    };

                    if (!coords.Any(a => a.Item1 == newCoord.Item1 && a.Item2 == newCoord.Item2))
                    {
                        coords.Add(newCoord);
                        coordExist = false;
                    }
                    else
                    {
                        coordExist = true;
                    }
                }

            }
            return coords;
        }
    }
}
