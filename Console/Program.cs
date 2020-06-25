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
        private List<GameBoard> _gameBoards = new List<GameBoard>();
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
             [X] Change _coordMapChar to use (x,y) instead of (y,x) as key
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

             [ ] Show info message on top always.. .eg 'Exit(Q) Mark(Enter) Move(Arrows)' and 'Player board' / 'Compouter board'
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

            SetupKeyHandlerFor(CurrentGameMode);
            _keyInputHandler.Listen();

            DisposeKeyHandlerEvents();
        }

        private void SetupKeyHandlerFor(GameMode gameMode)
        {
            _keyInputHandler = new KeyInputHandler()
                           .WithGameMode(gameMode);

            _keyInputHandler.ExitEvent += OnExit;
            _keyInputHandler.KeyActionEvent += OnKeyFired;
        }

        private void DisposeKeyHandlerEvents()
        {
            if (_keyInputHandler == null)
                return;

            _keyInputHandler.ExitEvent -= OnExit;
            _keyInputHandler.KeyActionEvent -= OnKeyFired;
        }

        private void OnExit(object sender, bool shouldExit)
        {
            if (!shouldExit)
                return;

            " ".OverwritePrintedChar();
            "Player decided to quit!".PrintGameMessage(false);
            Environment.Exit(0);
        }

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
                    KeyFiredInGamePlayMode(keyAction);
                    break;
                case GameMode.Exit:
                    break;
                default: break;
            }
        }

        private void StartGame()
        {

            _gameBoards.Add(new GameBoard(new Player("Player 1", PlayerType.Human)).WithShips(_ships));

            var shipGenerator = new ShipGenerator();
            var ships = shipGenerator.Generate();

            _gameBoards.Add(new GameBoard(new Player("Computer", PlayerType.Computer)).WithShips(ships.ToList()));

            // TODO: BoardPrinter should take GameBoards instead?
            // And should be able to print both compter and human
            var boardPrinter = new BoardPrinter();
            _ = boardPrinter.Print(new List<IShip>());

            "GAMEPLAY IS ON!!".PrintGameMessage();

            // TODO: Print board here.. for computer and human board..
        }

        #region Key Handling Game Play Mode
        private void KeyFiredInGamePlayMode(KeyAction keyAction)
        {
            switch (keyAction.Key.Value)
            {
                // case ConsoleKey.Enter:
                //     if (!HandleEnterKeyInSetupMode(keyAction))
                //         return;
                //     break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    // HandleArrowKeysInSetupMode(keyAction);
                    break;
                case ConsoleKey.S:
                    StartGame();
                    break;
                default:
                    return;
            }

            if (_coordMapChar.TryGetValue((keyAction.OldStepX, keyAction.OldStepY), out BoxContainer boxContainer))
            {
                System.Console.SetCursorPosition(keyAction.OldPostionX, keyAction.OldPositionY);
                boxContainer.BoxContent.Write(boxContainer.Color);
            }
            System.Console.SetCursorPosition(keyAction.NewPositionX, keyAction.NewPositionY);
            KeyConstants.Move.Write(Color.None);
        }

        #endregion

        #region Key Handling Setup Mode

        private bool IsAllShipsSetup() => ShipSetups.All(a => a.IsAllCoordsSet);
        private ShipSetup GetShipSetup() => ShipSetups.FirstOrDefault(f => !f.IsAllCoordsSet);
        private void KeyFiredInSetupMode(KeyAction keyAction)
        {
            if (IsAllShipsSetup())
            {
                CurrentGameMode = GameMode.GamePlay;
                _keyInputHandler.Mode = CurrentGameMode;
                "All ships are marked and validated. Press S to start the game!".PrintGameMessage();
                return;
            }

            switch (keyAction.Key.Value)
            {
                case ConsoleKey.Enter:
                    if (!HandleEnterKeyInSetupMode(keyAction))
                        return;
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.DownArrow:
                case ConsoleKey.LeftArrow:
                case ConsoleKey.RightArrow:
                    HandleArrowKeysInSetupMode(keyAction);
                    break;
                default:
                    return;
            }

            var shipSetup = GetShipSetup();
            $"Mark boxes for shiptype {shipSetup.ShipType.ToString()} ({shipSetup.Coords.Count}/{shipSetup.ShipType.NrOfBoxes()} boxes).."
                             .PrintGameMessage();
        }

        private void HandleArrowKeysInSetupMode(KeyAction keyAction)
        {
            if (_coordMapChar.TryGetValue((keyAction.OldStepX, keyAction.OldStepY), out BoxContainer boxContainer))
            {
                System.Console.SetCursorPosition(keyAction.OldPostionX, keyAction.OldPositionY);
                boxContainer.BoxContent.Write(boxContainer.Color);
            }
            System.Console.SetCursorPosition(keyAction.NewPositionX, keyAction.NewPositionY);
            KeyConstants.Move.Write(Color.None);
        }

        private bool HandleEnterKeyInSetupMode(KeyAction keyAction)
        {
            var setup = GetShipSetup();
            if (setup == null)
            {
                throw new Exception("No setup found but indication that setup is done. Fatal error!");
            }

            MarkCoordinateWithShip(keyAction, setup);

            if (setup.IsAllCoordsSet)
            {
                try
                {
                    var ship = _shipFactory.Build(setup.ShipType, setup.Coords);
                    _ships.Add(ship);
                }
                catch (ShipValidationException exception)
                {
                    HandleShipSetupNotValidated(setup, keyAction, exception.Message);
                    return false;
                }
            }

            if (!IsAllShipsSetup())
            {
                var nextSetup = GetShipSetup();
                $"Mark boxes for shiptype {nextSetup.ShipType.ToString()} ({nextSetup.Coords.Count}/{nextSetup.ShipType.NrOfBoxes()} boxes).."
                    .PrintGameMessage();
            }

            return false;
        }

        private void MarkCoordinateWithShip(KeyAction keyAction, ShipSetup setup)
        {
            System.Console.SetCursorPosition(keyAction.NewPositionX, keyAction.NewPositionY);
            "✔".Write(setup.ShipType.GetColor());
            setup.AddCoord((Column)keyAction.OldStepX, keyAction.OldStepY);
            setup.AddPosition(keyAction.NewPositionX, keyAction.NewPositionY);
            _coordMapChar[(keyAction.OldStepX, keyAction.OldStepY)] = new BoxContainer("✔", setup.ShipType.GetColor());
        }

        private void HandleShipSetupNotValidated(ShipSetup setup, KeyAction keyAction, string exceptionMessage)
        {
            foreach (var coord in setup.Coords)
            {
                _coordMapChar[((int)coord.column, coord.row)] = new BoxContainer().Empty();
            }
            setup.Clear(keyAction.NewPositionX, keyAction.NewPositionY);
            exceptionMessage.PrintGameMessage();
        }

        #endregion

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
