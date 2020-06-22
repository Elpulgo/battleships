using System;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using Console.Models;
using Core.Models.Ships;

namespace Console
{
    class Program
    {
        private string LastBoxChar { get; set; } = string.Empty;
        private List<IShip> _ships = new List<IShip>();
        private static Dictionary<(int, int), string> _coordMapChar = new Dictionary<(int, int), string>();

        public KeyInputHandler _keyInputHandler;
        static void Main(string[] args)
        {

            // TODO:
            /*
             [] Eventloop for key input 
                  [X] Exit
                  [X] Move
                  [ ] Enter(hit/mark)
                  [ ] P (Place mark)?
                  [ ] F (Finish place ship)?
                  [ ] S (Start game)? Only when all ships finished
             [X] Use PositionState interface instead of x properties in KeyInputHandler

             [ ] GameMode -> Setup/Play
             [ ] Add writer, Extention ShipWriter in console proj. And writer to write the different marks (* / x etc..)

             [X] Interface for Ship/ShipContainer (rename?)
             [ ] Utility to create coord key => pass column, row, get key entity. Use everywhere so
                 built in the same way
             [X] Abstract Factory to create ships
                  [X] Color
                  [X] Boxes
                  [X] Name

             [ ] GameEngine/Manager/GamePlay?
                  [ ] Fire Action (include playerid)-> Return ActionResult containing:
                      [ ] Coord (x,y)
                      [ ] ShipWasHit
                      [ ] ShipWasSunk
                      [ ] Message To Display e.g "Player missed.", "Player hit! Destroyer was sunk.", "Player won!"
                      [ ] Player
            */

            var pro = new Program();
            pro.Run();
        }

        public void Run()
        {
            // Need to be done before keyInputHandler is created.
            // Since handler calculate positions after board
            // which is created in this method.
            CreateShipsForPlayer();

            _keyInputHandler = new KeyInputHandler()
                .WithGameMode(GameMode.Setup);

            _keyInputHandler.ExitEvent += OnExit;
            _keyInputHandler.KeyActionEvent += OnKeyFired;

            _keyInputHandler.Listen();
        }

        private static void PrintMessage(string message, bool bringBackCursor = true)
        {
            int currentLeft = System.Console.CursorLeft;
            int currentTop = System.Console.CursorTop;

            System.Console.SetCursorPosition(0, System.Console.BufferHeight);
            System.Console.Write(message);
            if (bringBackCursor)
            {
                System.Console.SetCursorPosition(currentLeft, currentTop);
            }
        }

        private void OverwritePrintedChar()
        {
            int currentLeft = System.Console.CursorLeft;
            int currentTop = System.Console.CursorTop;

            System.Console.SetCursorPosition(currentLeft - 1, currentTop);

            System.Console.Write(" ");
            System.Console.SetCursorPosition(currentLeft, currentTop);
        }

        private void OnExit(object sender, bool shouldExit)
        {
            if (shouldExit)
            {
                OverwritePrintedChar();
                PrintMessage("Player decided to quit!", false);
                Environment.Exit(0);
            }
        }

        private void OnKeyFired(object sender, KeyAction keyAction)
        {
            if (_coordMapChar.TryGetValue((keyAction.OldStepY, keyAction.OldStepX), out string oldChar))
            {
                System.Console.SetCursorPosition(keyAction.OldPostionX, keyAction.OldPositionY);
                System.Console.Write(oldChar);
            }
            System.Console.SetCursorPosition(keyAction.NewPositionX, keyAction.NewPositionY);

            // TODO: Should also indicate on game mode and so on what should be written..
            System.Console.Write("*");
        }

        private void CreateShipsForPlayer()
        {
            var shipGenerator = new ShipGenerator();
            var ships = shipGenerator.Generate();
            _ships = ships.ToList();

            // PrintInfoAboutShips(_ships);

            var boardPrinter = new BoardPrinter();
            _coordMapChar = boardPrinter.Print(_ships);
        }

        private void PrintInfoAboutShips(List<IShip> ships)
        {
            foreach (var ship in ships)
            {
                System.Console.WriteLine($"Shiptype: {ship.Name}\t IsDestroyed: {ship.IsDestroyed}");
                foreach (var coord in ship.Coordinates)
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
