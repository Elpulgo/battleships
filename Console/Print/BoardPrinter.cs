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
        private static string HorizontalDivier = "   ----------------------------------------";
        public BoardPrinter() { }


        public (Dictionary<(int x, int y), BoxContainer> coordMap_Human, Dictionary<(int x, int y), BoxContainer> coordMap_Computer) PrintMultipleBoards(
            List<IShip> humanShips, List<IShip> computerShips)
        {

            // TODO: Computer board to LEFT where we navigate and human board to RIGHT where we display..
            var coordinateMapChar_Human = new Dictionary<(int x, int y), BoxContainer>();
            var coordinateMapChar_Computer = new Dictionary<(int x, int y), BoxContainer>();

            var shipCoordinatesMap_Human = humanShips
                .SelectMany(s => s.Coordinates.Select(s => s))
                .ToDictionary(d => d.Key);

            var shipCoordinatesMap_Computer = computerShips
                .SelectMany(s => s.Coordinates.Select(s => s))
                .ToDictionary(d => d.Key);

            var coordinateColorMap_Human = GroupCoordinatesByColor(humanShips);
            var coordinateColorMap_Computer = GroupCoordinatesByColor(computerShips);

            PrintColumns(true);

            foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
            {
                // FOR COMPUTER SHIPS
                System.Console.Write($"{row}");
                PrintRowDigitDivider(row);

                foreach (CoordinatesHelper.Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
                {
                    if (shipCoordinatesMap_Computer.TryGetValue(CoordinateKey.Build(column, row), out CoordinateContainer coord))
                    {
                        var color = Color.None;
                        coordinateColorMap_Computer.TryGetValue(coord, out color);

                        $" {KeyConstants.Ship} ".Write(color);

                        coordinateMapChar_Computer[((int)column, row)] = new BoxContainer(KeyConstants.Ship, color);
                        System.Console.Write("|");
                        continue;
                    }

                    coordinateMapChar_Computer[((int)column, row)] = new BoxContainer().Empty();

                    System.Console.Write("   ");
                    System.Console.Write("|");

                }
                // PrintDivider();


                // FOR HUMAN SHIPS

                System.Console.Write("\t\t");
                System.Console.Write($"{row}");
                PrintRowDigitDivider(row);
                foreach (CoordinatesHelper.Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
                {
                    if (shipCoordinatesMap_Human.TryGetValue(CoordinateKey.Build(column, row), out CoordinateContainer coord))
                    {
                        var color = Color.None;
                        coordinateColorMap_Human.TryGetValue(coord, out color);

                        $" {KeyConstants.Ship} ".Write(color);

                        coordinateMapChar_Human[((int)column, row)] = new BoxContainer(KeyConstants.Ship, color);
                        System.Console.Write("|");
                        continue;
                    }

                    coordinateMapChar_Human[((int)column, row)] = new BoxContainer().Empty();

                    System.Console.Write("   ");
                    System.Console.Write("|");

                }
                PrintDivider(true);

            }

            return (coordinateMapChar_Human, coordinateMapChar_Computer);
        }
        public Dictionary<(int x, int y), BoxContainer> Print(List<IShip> ships)
        {
            var coordinateMapChar = new Dictionary<(int x, int y), BoxContainer>();

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

                        $" {KeyConstants.Ship} ".Write(color);

                        coordinateMapChar[((int)column, row)] = new BoxContainer(KeyConstants.Ship, color);
                        System.Console.Write("|");
                        continue;
                    }

                    coordinateMapChar[((int)column, row)] = new BoxContainer().Empty();

                    System.Console.Write("   ");
                    System.Console.Write("|");

                }
                PrintDivider();
            }

            return coordinateMapChar;
        }

        private void PrintColumns(bool multiple = false)
        {
            var columns = "    A | B | C | D | E | F | G | H | I | J |";

            if (!multiple)
            {
                System.Console.WriteLine(columns);
                System.Console.WriteLine(HorizontalDivier);
                return;
            }

            var line = $"{columns}\t\t{columns}";

            System.Console.WriteLine(line);
            System.Console.WriteLine($"{HorizontalDivier}\t\t{HorizontalDivier}");
        }

        private void PrintDivider(bool multiple = false)
        {
            if (!multiple)
            {
                System.Console.WriteLine();
                System.Console.WriteLine(HorizontalDivier);
                return;
            }

            var line = $"{HorizontalDivier}\t\t{HorizontalDivier}";
            System.Console.WriteLine();
            System.Console.WriteLine(line);
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

