using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace Console
{
    public class BoardPrinter
    {
        // private readonly List<ShipContainer> _shipContainers;
        private static string HorizontalDivier = "   ----------------------------------------";
        private static string ShipIcon = " ✔ ";
        // private static string MarkIcon = " ✘ ";

        public BoardPrinter() { }

        public Dictionary<(int, int), string> Print(List<ShipContainer> shipContainers)
        {
            var coordinateMapChar = new Dictionary<(int, int), string>();

            var shipCoordinates = shipContainers
                            .SelectMany(s => s.Coordinates.Select(s => s))
                            .ToDictionary(d => d.Key);

            PrintColumns();

            foreach (var row in Enumerable.Range(1, CoordinatesHelper.GetRowCount()))
            {
                System.Console.Write($"{row}");
                PrintRowDigitDivider(row);

                foreach (CoordinatesHelper.Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
                {
                    // // FOR TESTING
                    // if (column == CoordinatesHelper.Column.C && shipCoordinates.ContainsKey($"{column.ToString()}{row}"))
                    // {
                    //     System.Console.ForegroundColor = ConsoleColor.Red;

                    //     System.Console.Write(ShipIcon);
                    //     System.Console.ResetColor();
                    //     System.Console.Write("|");

                    //     continue;
                    // }

                    if (shipCoordinates.TryGetValue($"{column.ToString()}{row}", out CoordinateContainer coord))
                    {

                        // TODO: Add shipcolors?
                        System.Console.ForegroundColor = coord.IsMarked && coord.HasShip ? ConsoleColor.Red : ConsoleColor.Green;

                        System.Console.Write(ShipIcon);
                        coordinateMapChar[(row, (int)column)] = "✔";
                        System.Console.ResetColor();
                        System.Console.Write("|");
                        continue;
                    }
                    // // For testing!!
                    // else if (row == 10)
                    // {
                    //     System.Console.Write(MarkIcon);
                    // }

                    coordinateMapChar[(row, (int)column)] = " ";

                    System.Console.Write("   ");
                    System.Console.Write("|");

                }
                PrintDivider();
            }

            return coordinateMapChar;
        }

        private void PrintColumns()
        {
            System.Console.WriteLine("    A | B | C | D | E | F | G | H | I | J |");
            System.Console.WriteLine(HorizontalDivier);
        }

        private void PrintDivider()
        {
            System.Console.WriteLine();
            System.Console.WriteLine(HorizontalDivier);
        }

        private static void PrintRowDigitDivider(int row)
        {
            if (row < 10)
            {
                System.Console.Write(" |");
                return;
            }

            System.Console.Write("|");
        }
    }
}

