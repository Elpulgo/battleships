using System;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using Console.Models;
using Core.Models.Ships;
using Console.Print;
using static Core.Models.CoordinatesHelper;
using Core.Factories;

namespace Console
{
    class Program
    {
        private string LastBoxChar { get; set; } = string.Empty;
        private GameMode CurrentGameMode { get; set; }
        private List<ShipSetup> ShipSetups { get; set; } = new List<ShipSetup>();
        private List<IShip> _ships = new List<IShip>();

        private readonly ShipFactory _shipFactory = new ShipFactory();

        private static Dictionary<(int x, int y), BoxContainer> _coordMapChar = new Dictionary<(int x, int y), BoxContainer>();

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
             [ ] Change _coordMapChar to use (x,y) instead of (y,x) as key
             [X] ShipBuildException should check so all ships are in line too. Do in ShipBase
             [ ] GameMode -> Setup/Play
             [X] Add writer, Extention ShipWriter in console proj. And writer to write the different marks (* / x etc..)

             [X] Interface for Ship/ShipContainer (rename?)
             [X] Utility to create coord key => pass column, row, get key entity. Use everywhere so
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
            CurrentGameMode = GameMode.Setup;

            // Need to be done before keyInputHandler is created.
            // Since handler calculate positions after board
            // which is created in this method.
            PrintBoard();

            ShipSetups = ShipConstants.GetShipTypesPerPlayer()
                .Select(s => new ShipSetup(s))
                .ToList();

            _keyInputHandler = new KeyInputHandler()
                .WithGameMode(CurrentGameMode);

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

        private void OverwritePrintedChar(string message, Color color = Color.None)
        {
            int currentLeft = System.Console.CursorLeft;
            int currentTop = System.Console.CursorTop;

            System.Console.SetCursorPosition(currentLeft - 1, currentTop);

            message.Write(color);
            System.Console.SetCursorPosition(currentLeft, currentTop);
        }

        private void OnExit(object sender, bool shouldExit)
        {
            if (shouldExit)
            {
                OverwritePrintedChar(" ");
                PrintMessage("Player decided to quit!", false);
                Environment.Exit(0);
            }
        }

        private (bool allDone, ShipSetup setup) GetShipSetup()
            => (ShipSetups.All(a => a.IsAllCoordsSet), ShipSetups.FirstOrDefault(f => !f.IsAllCoordsSet));

        private void OnKeyFired(object sender, KeyAction keyAction)
        {
            if (!keyAction.Key.HasValue)
                return;

            switch (CurrentGameMode)
            {
                case GameMode.Setup:
                    KeyFiredInSetupMode(keyAction);
                    break;

                case GameMode.GamePlay:
                    // TODO: Refactor to nice method.........
                    if (_coordMapChar.TryGetValue((keyAction.OldStepX, keyAction.OldStepY), out BoxContainer boxContainer))
                    {
                        System.Console.SetCursorPosition(keyAction.OldPostionX, keyAction.OldPositionY);
                        boxContainer.BoxContent.Write(boxContainer.Color);
                    }
                    System.Console.SetCursorPosition(keyAction.NewPositionX, keyAction.NewPositionY);
                    System.Console.Write("*");
                    break;
                case GameMode.Exit:
                    break;
                default: break;
            }
        }

        private void KeyFiredInSetupMode(KeyAction keyAction)
        {
            var (allDone, setup) = GetShipSetup();

            if (allDone)
            {
                PrintMessage("All ships are marked and validated. Press S to start the game!");
                return;
            }

            if (keyAction.Key.Value == ConsoleKey.Enter)
            {
                System.Console.SetCursorPosition(keyAction.NewPositionX, keyAction.NewPositionY);
                "✔".Write(setup.ShipType.GetColor());
                setup.SetCoord((Column)keyAction.OldStepX, keyAction.OldStepY);
                _coordMapChar[(keyAction.OldStepX, keyAction.OldStepY)] = new BoxContainer("✔", setup.ShipType.GetColor());

                if (setup.IsAllCoordsSet)
                {
                    try
                    {
                        var ship = _shipFactory.Build(setup.ShipType, setup.Coords);
                    }
                    catch (ShipValidationException exception)
                    {
                        // ShipBase must check so all coords are in line too
                        PrintMessage(exception.Message);
                        return;
                    }
                    // TODO: Create ship if setup.AllCordsSet
                    // and validate the ship before adding it to list.
                    // Catch ShipValidationException otherwise and clear the coords for ship
                    // and on the coordMapChar
                }

                (allDone, setup) = GetShipSetup();
                if (!allDone)
                {
                    PrintMessage($"Mark boxes for shiptype {setup.ShipType.ToString()}({setup.Coords.Count}/{setup.ShipType.NrOfBoxes()} boxes).");
                }

                return;
            }

            PrintMessage($"Mark boxes for shiptype {setup.ShipType.ToString()}({setup.Coords.Count}/{setup.ShipType.NrOfBoxes()} boxes).");

            if (_coordMapChar.TryGetValue((keyAction.OldStepX, keyAction.OldStepY), out BoxContainer boxContainer))
            {
                System.Console.SetCursorPosition(keyAction.OldPostionX, keyAction.OldPositionY);
                boxContainer.BoxContent.Write(boxContainer.Color);
            }
            System.Console.SetCursorPosition(keyAction.NewPositionX, keyAction.NewPositionY);
            System.Console.Write("*");
        }

        private void PrintBoard()
        {
            // var shipGenerator = new ShipGenerator();
            // var ships = shipGenerator.Generate();
            // _ships = ships.ToList();

            // PrintInfoAboutShips(_ships);

            var boardPrinter = new BoardPrinter();
            _coordMapChar = boardPrinter.Print(new List<IShip>());
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
