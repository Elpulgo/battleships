using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace Console
{
    public class BoardPrinter
    {
        private readonly List<ShipContainer> _shipContainers;
        private static string HorizontalDivier = "   ----------------------------------------";
        private static string ShipIcon = " ✔ ";
        private static string MarkIcon = " ✘ ";

        public BoardPrinter() { }

        public void Print(List<ShipContainer> shipContainers)
        {
            var flattenedCoordKeys = shipContainers
                            .SelectMany(s => s.Coordinates.Select(p => p.Key))
                            .ToDictionary(d => d);

            PrintColumns();

            foreach (var row in Enumerable.Range(1, CoordinatesHelper.GetRowCount()))
            {
                System.Console.Write($"{row}");
                PrintRowDigitDivider(row);

                foreach (CoordinatesHelper.Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
                {
                    // FOR TESTING
                    if (column == CoordinatesHelper.Column.C && flattenedCoordKeys.ContainsKey($"{column.ToString()}{row}"))
                    {
                        System.Console.ForegroundColor = ConsoleColor.Red;

                        System.Console.Write(ShipIcon);
                        System.Console.ResetColor();
                        System.Console.Write("|");

                        continue;
                    }

                    if (flattenedCoordKeys.ContainsKey($"{column.ToString()}{row}"))
                    {
                        System.Console.ForegroundColor = ConsoleColor.Green;

                        System.Console.Write(ShipIcon);
                        // ☑
                        // ✔
                        System.Console.ResetColor();
                    }
                    // For testing!!
                    else if (row == 10)
                    {
                        System.Console.Write(MarkIcon);
                    }
                    else
                    {
                        System.Console.Write("   ");
                    }

                    System.Console.Write("|");

                }
                PrintDivider();
            }
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

