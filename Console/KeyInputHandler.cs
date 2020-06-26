using System;
using Core.Models;
using System.Threading;
using Console.Models;

namespace Console
{
    public class KeyInputHandler
    {
        #region Members

        private PositionSate _positionState;
        private int _initBufferHeight;
        private int _initBufferWidth;

        #endregion

        public event EventHandler<bool> ExitEvent;

        public event EventHandler<KeyAction> KeyActionEvent;

        public GameMode Mode { get; private set; }

        public KeyInputHandler()
        {
            SetupBufferHeight();
            _positionState = new PositionSate(
                System.Console.CursorLeft + 4,
                System.Console.CursorTop - 2
            );
            SetInitCursorPositions();
        }

        public KeyInputHandler WithGameMode(GameMode mode)
        {
            Mode = mode;
            return this;
        }

        public void SetGameMode(GameMode mode)
        {
            ClearScreen();
            Mode = mode;
        }

        private void ClearScreen()
        {
            // System.Console.WriteLine("buffer is: " + _initPosition_Y);
            // for (int i = 1; i < _initBufferHeight; i++)
            // {
            //     System.Console.SetCursorPosition(0, i);
            //     System.Console.WriteLine();
            // }

            // System.Console.SetCursorPosition(0, _initPosition_Y);
        }

        public void Listen()
        {
            FireKeyActionEvent();

            while (Mode == GameMode.Setup || Mode == GameMode.GamePlay)
            {
                var key = ListenForInput();
                _positionState.Validate();
                FireKeyActionEvent(key);
                Thread.Sleep(10);
            }
        }

        private ConsoleKey? ListenForInput()
        {
            if (_initBufferHeight != System.Console.BufferHeight || _initBufferWidth != System.Console.BufferWidth)
            {
                // TODO: Was resized, start over and print board again..
                System.Console.Clear();
                // return false;
            }

            _positionState.CacheCurrentState();

            var command = System.Console.ReadKey().Key;

            switch (command)
            {
                case ConsoleKey.DownArrow:
                    _positionState.Increment_Y();
                    return command;
                case ConsoleKey.UpArrow:
                    _positionState.Decrement_Y();
                    return command;
                case ConsoleKey.LeftArrow:
                    _positionState.Decrement_X();
                    return command;
                case ConsoleKey.RightArrow:
                    _positionState.Increment_X();
                    return command;
                case ConsoleKey.Q:
                    ExitEvent.Invoke(this, true);
                    Mode = GameMode.Exit;
                    return command;
                case ConsoleKey.Enter:
                case ConsoleKey.S:
                    return command;
                default: break;
            }

            return null;
        }

        private void SetInitCursorPositions()
        {
            if (_positionState.Position_X > _initBufferWidth && _positionState.Position_Y > _initBufferHeight)
            {
                System.Console.SetCursorPosition(System.Console.CursorLeft + 4, System.Console.CursorTop);
                return;
            }

            System.Console.SetCursorPosition(_positionState.Position_X, _positionState.Position_Y);
        }

        private void FireKeyActionEvent(ConsoleKey? key = null)
        {
            if (key.HasValue && key.Value == ConsoleKey.Q)
                return;

            var keyAction = new KeyAction(key)
                .WithOldStep(_positionState.OldStep_X, _positionState.OldStep_Y)
                .WithOldPostion(_positionState.OldPosition_X, _positionState.OldPosition_Y)
                .WithNewPosition(_positionState.Position_X, _positionState.Position_Y);

            KeyActionEvent.Invoke(this, keyAction);
        }

        private void SetupBufferHeight()
        {
            _initBufferHeight = System.Console.BufferHeight;
            _initBufferWidth = System.Console.BufferWidth;
        }
    }
}
