using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Models.Ships;
using Core.Utilities;

namespace Console.Print
{
    public class BoardPrinter
    {
        // private readonly List<ShipContainer> _shipContainers;
        private static string HorizontalDivier = "   ----------------------------------------";
        private static string ShipIcon = "✔";
        // private static string MarkIcon = " ✘ ";

        public BoardPrinter() { }

        public Dictionary<(int, int), BoxContainer> Print(List<IShip> ships)
        {
            var coordinateMapChar = new Dictionary<(int, int), BoxContainer>();

            var shipCoordinatesMap = ships
                            .SelectMany(s => s.Coordinates.Select(s => s))
                            .ToDictionary(d => d.Key);

            var coordinateColorMap = GroupCoordinatesByColor(ships);
            PrintColumns();

            foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
            {
                System.Console.Write($"{row}");
                PrintRowDigitDivider(row);

                foreach (CoordinatesHelper.Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
                {
                    if (shipCoordinatesMap.TryGetValue(CoordinateKey.Build(column, row), out CoordinateContainer coord))
                    {
                        var color = Color.None;
                        coordinateColorMap.TryGetValue(coord, out color);

                        // TODO: Add shipcolors?
                        System.Console.ForegroundColor = coord.IsMarked && coord.HasShip ? ConsoleColor.Red : ConsoleColor.Green;

                        $" {ShipIcon} ".Write(color);

                        coordinateMapChar[(row, (int)column)] = new BoxContainer("✔", color);
                        System.Console.Write("|");
                        continue;
                    }

                    coordinateMapChar[(row, (int)column)] = new BoxContainer().Empty();

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

        private Dictionary<CoordinateContainer, Color> GroupCoordinatesByColor(List<IShip> ships)
        {
            var coordinateColorMap = new Dictionary<CoordinateContainer, Color>();

            foreach (var ship in ships)
            {
                foreach (var coord in ship.Coordinates)
                {
                    coordinateColorMap.TryAdd(coord, ship.Color);
                }
            }

            return coordinateColorMap;
        }
    }
}

