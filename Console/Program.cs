using System;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using static Core.Models.CoordinatesHelper;

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
            var shipContainers = new List<ShipContainer>();
            foreach (var ship in ShipConstants.GetShipTypesPerPlayer())
            {
                var shipContainer = new ShipContainer(ship);
                var coords = GenerateCoordinates(shipContainer.Boxes);
                shipContainer.SetCoordinates(coords.ToList());
                shipContainers.Add(shipContainer);
            }


            foreach (var item in shipContainers)
            {
                System.Console.WriteLine($"Shiptype: {item.ShipType}\t IsDestroyed: {item.IsDestroyed}");
                foreach (var coord in item.Coordinates)
                {
                    System.Console.WriteLine($"\tCoord: {coord.Key}\t IsMarked: {coord.IsMarked} \t Has ship: {coord.HasShip}");
                }
            }
            
        }

        private IEnumerable<(Column column, int row)> GenerateCoordinates(int boxes)
        {

            var random = new Random();

            foreach (var box in Enumerable.Range(1, boxes))
            {
                var nextRandom = random.Next(1, 10);
                yield return ((CoordinatesHelper.Column)nextRandom, nextRandom);
            }
        }
    }
}
