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

            System.Console.WriteLine();
            System.Console.WriteLine("-----------------------------------");
            System.Console.WriteLine();

            var boardPrinter = new BoardPrinter();
            boardPrinter.Print(shipContainers.ToList());


            // var flattenedCordKeys = shipContainers
            //     .SelectMany(s => s.Coordinates.Select(p => p.Key))
            //     .ToDictionary(d => d);

            // // System.Console.WriteLine("\tA\tB\tC\tD\tE\tF\tG\tH\tI\tJ\t");
            // System.Console.WriteLine("    A | B | C | D | E | F | G | H | I | J |");
            // System.Console.WriteLine("   ----------------------------------------");


            // foreach (var row in Enumerable.Range(1, CoordinatesHelper.GetRowCount()))
            // {
            //     // System.Console.Write($"{row}\t");
            //     System.Console.Write($"{row}");
            //     if (row < 10)
            //     {
            //         System.Console.Write(" |");
            //     }
            //     else
            //     {
            //         System.Console.Write("|");
            //     }

            //     foreach (CoordinatesHelper.Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
            //     {
            //         // FOR TESTING
            //         if (column == CoordinatesHelper.Column.C && flattenedCordKeys.ContainsKey($"{column.ToString()}{row}"))
            //         {
            //             System.Console.ForegroundColor = ConsoleColor.Red;

            //             System.Console.Write(" ✔ ");
            //             System.Console.ResetColor();
            //             System.Console.Write("|");

            //             continue;
            //         }

            //         if (flattenedCordKeys.ContainsKey($"{column.ToString()}{row}"))
            //         {
            //             System.Console.ForegroundColor = ConsoleColor.Green;

            //             System.Console.Write(" ✔ ");
            //             // ☑
            //             // ✔
            //             System.Console.ResetColor();
            //         }
            //         // For testing!!
            //         else if (row == 10)
            //         {
            //             System.Console.Write(" ✘ ");
            //         }
            //         else
            //         {
            //             System.Console.Write("   ");
            //         }

            //         System.Console.Write("|");

            //     }

            //     System.Console.WriteLine();
            //     System.Console.WriteLine("   ----------------------------------------");

            // }



        }
    }
}
