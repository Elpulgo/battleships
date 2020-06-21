using System;
using Core.Models;
using System.Collections.Generic;
using System.Linq;
using static Core.Models.CoordinatesHelper;
using Core.Utilities;
using System.Threading;
using Console.Models;

namespace Console
{
    public class KeyInputHandler
    {
        private const int MaxYSteps = 10;
        private const int MaxXSteps = 10;

        #region Members


        // TOOO: Use PositionState
        private int _position_Y;
        private int _position_X;

        private int _oldPosition_Y;
        private int _oldPosition_X;
        private int _initBufferHeight;
        private int _initBufferWidth;
        private int _steps_X = 1;
        private int _steps_Y = 10;
        private int _oldSteps_X = 1;
        private int _oldSteps_Y = 10;

        #endregion

        public event EventHandler<bool> ExitEvent;
        public event EventHandler<KeyAction> KeyActionEvent;

        public GameMode Mode { get; set; }

        public KeyInputHandler()
        {
            SetupBufferHeight();
            SetupPositions();
            SetInitCursorPositions();
        }

        public KeyInputHandler WithGameMode(GameMode mode)
        {
            Mode = mode;
            return this;
        }

        public void Listen()
        {
            FireKeyActionEvent();

            // TODO: Change and handle GamePlay mode here..
            while (Mode == GameMode.Setup)
            {
                ListenForInput();
                ValidateCurrentCursorPosition();
                FireKeyActionEvent();
                Thread.Sleep(10);
            }
        }

        private void ListenForInput()
        {
            if (_initBufferHeight != System.Console.BufferHeight || _initBufferWidth != System.Console.BufferWidth)
            {
                // TODO: Was resized, start over and print board again..
                System.Console.Clear();
                // return false;
            }

            if (_steps_Y < 5)
            {

                int currentLeft = System.Console.CursorLeft;
                int currentTop = System.Console.CursorTop;

                System.Console.SetCursorPosition(0, System.Console.BufferHeight);
                System.Console.Write($"Y step right now is: {_steps_Y}");
                System.Console.SetCursorPosition(currentLeft, currentTop);
                // PrintMessage($"Y step right now is: {_ySteps}");
            }

            CacheOldPositions();

            var command = System.Console.ReadKey().Key;

            switch (command)
            {
                case ConsoleKey.DownArrow:
                    DownArrow();
                    break;
                case ConsoleKey.UpArrow:
                    UpArrow();
                    break;
                case ConsoleKey.LeftArrow:
                    LeftArrow();
                    break;
                case ConsoleKey.RightArrow:
                    RightArrow();
                    break;
                case ConsoleKey.Q:
                    ExitEvent.Invoke(this, true);
                    Mode = GameMode.Exit;
                    break;
                default: break;
            }

            void UpArrow()
            {
                if (_steps_Y > 1 && _position_Y > 1)
                {
                    _position_Y = _position_Y - 2;
                    _steps_Y--;
                }
            }
            void DownArrow()
            {
                if (_steps_Y < MaxYSteps)
                {
                    _position_Y = _position_Y + 2;
                    _steps_Y++;
                }
            }
            void LeftArrow()
            {
                if (_steps_X > 1 && _position_X > 3)
                {
                    _position_X = _position_X - 4;
                    _steps_X--;
                }
            }
            void RightArrow()
            {
                if (_steps_X < MaxXSteps)
                {
                    _position_X = _position_X + 4;
                    _steps_X++;
                }
            }
        }

        private void SetInitCursorPositions()
        {
            if (_position_X > _initBufferWidth && _position_Y > _initBufferHeight)
            {
                System.Console.SetCursorPosition(System.Console.CursorLeft + 4, System.Console.CursorTop);
                return;
            }

            System.Console.SetCursorPosition(_position_X, _position_Y);
        }

        private void ValidateCurrentCursorPosition()
        {
            if (_position_X > System.Console.BufferWidth || _position_Y > System.Console.BufferHeight)
            {
                System.Console.Clear();
                System.Console.WriteLine("Coords are greater than buffer!");
            }
        }
        private void FireKeyActionEvent()
        {
            var keyAction = new KeyAction()
                .WithOldStep(_oldSteps_X, _oldSteps_Y)
                .WithOldPostion(_oldPosition_X, _oldPosition_Y)
                .WithNewPosition(_position_X, _position_Y);

            KeyActionEvent.Invoke(this, keyAction);
        }

        private void SetupBufferHeight()
        {
            _initBufferHeight = System.Console.BufferHeight;
            _initBufferWidth = System.Console.BufferWidth;
        }

        private void SetupPositions()
        {
            _position_X = System.Console.CursorLeft + 4;
            _position_Y = System.Console.CursorTop - 2;
            _oldPosition_X = _position_X;
            _oldPosition_Y = _position_Y;
        }

        private void CacheOldPositions()
        {
            _oldSteps_X = _steps_X;
            _oldSteps_Y = _steps_Y;
            _oldPosition_Y = _position_Y;
            _oldPosition_X = _position_X;
        }
    }
}
