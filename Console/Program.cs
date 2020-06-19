using System;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using static Core.Models.CoordinatesHelper;
using Core.Utilities;
using System.Threading;

namespace Console
{
    class Program
    {
        private string LastBoxChar { get; set; } = string.Empty;
        private List<ShipContainer> _shipContainers = new List<ShipContainer>();
        private static Dictionary<(int, int), string> _coordMapChar = new Dictionary<(int, int), string>();
        static void Main(string[] args)
        {
            var pro = new Program();
            var exit = false;

            // while (!exit)
            // {
            //     System.Console.WriteLine(System.Console.BufferHeight);
            //     System.Console.WriteLine(System.Console.BufferWidth);
            //     Thread.Sleep(3000);
            //     exit = System.Console.ReadKey().Key == ConsoleKey.Q;
            // }

            while (!exit)
            {
                System.Console.Clear();
                pro.CreateShipsForPlayer();
                exit = GamePlay();
            }

            System.Console.Clear();
            System.Console.WriteLine("Will exit!");
        }

        private static void PrintMessage(string message)
        {
            int currentLeft = System.Console.CursorLeft;
            int currentTop = System.Console.CursorTop;

            System.Console.SetCursorPosition(0, System.Console.BufferHeight);
            System.Console.Write(message);
            System.Console.SetCursorPosition(currentLeft, currentTop);
        }

        private static bool GamePlay()
        {
            var cursorLeft = System.Console.CursorLeft + 4;
            var cursorTop = System.Console.CursorTop - 2;

            var initBufferHeight = System.Console.BufferHeight;
            var initBufferWidth = System.Console.BufferWidth;


            if (cursorLeft > System.Console.BufferWidth && cursorTop > System.Console.BufferHeight)
            {
                System.Console.WriteLine("Init coords is greater than buff");
                System.Console.SetCursorPosition(cursorLeft + 4, cursorTop);
            }
            else
            {
                System.Console.SetCursorPosition(cursorLeft, cursorTop);
            }

            var exit = false;
            int x = cursorLeft, y = cursorTop;
            System.Console.Write("*");
            int ySteps = 10;
            int xSteps = 1;
            int maxYSteps = 10;
            int maxXSteps = 10;
            while (!exit)
            {
                if (initBufferHeight != System.Console.BufferHeight || initBufferWidth != System.Console.BufferWidth)
                {
                    System.Console.Clear();
                    return false;
                }

                if (ySteps < 5)
                {
                    PrintMessage($"Y step right now is: {ySteps}");
                }

                var oldXStep = xSteps;
                var oldYStep = ySteps;
                var oldY = y;
                var oldX = x;
                var command = System.Console.ReadKey().Key;
                switch (command)
                {
                    case ConsoleKey.DownArrow:
                        {
                            if (ySteps < maxYSteps)
                            {
                                y = y + 2;
                                ySteps++;
                            }
                            break;
                        }
                    case ConsoleKey.UpArrow:
                        {
                            if (ySteps > 1 && y > 1)
                            {
                                y = y - 2;
                                ySteps--;
                            }
                            break;
                        }
                    case ConsoleKey.LeftArrow:
                        {
                            if (xSteps > 1 && x > 3)
                            {
                                x = x - 4;
                                xSteps--;
                            }
                            break;
                        }
                    case ConsoleKey.RightArrow:
                        {
                            if (xSteps < maxXSteps)
                            {
                                x = x + 4;
                                xSteps++;
                            }
                            break;
                        }
                    case ConsoleKey.Q:
                        {
                            return true;
                        }
                    default: break;
                }

                if (x > System.Console.BufferWidth || y > System.Console.BufferHeight)
                {
                    System.Console.WriteLine("Coords are greater than buffer!");
                    System.Console.Clear();
                    return false;
                }
                else
                {
                    if (_coordMapChar.TryGetValue((oldYStep, oldXStep), out string oldChar))
                    {
                        System.Console.SetCursorPosition(oldX, oldY);
                        System.Console.Write(oldChar);
                    }
                    System.Console.SetCursorPosition(x, y);
                    System.Console.Write("*");
                }

                Thread.Sleep(10);
            }

            return true;
        }

        private void CreateShipsForPlayer()
        {
            var shipGenerator = new ShipGenerator();
            var shipContainers = shipGenerator.Generate();
            _shipContainers = shipContainers.ToList();

            // PrintInfoAboutContainers(_shipContainers);

            var boardPrinter = new BoardPrinter();
            _coordMapChar = boardPrinter.Print(shipContainers.ToList());
        }

        private void PrintInfoAboutContainers(List<ShipContainer> shipContainers)
        {
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
        }
    }
}
