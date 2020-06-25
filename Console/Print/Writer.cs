using System;
using Core.Models.Ships;

namespace Console.Print
{
    public static class Writer
    {
        public static void Write(this string message, Color color)
        {
            System.Console.ForegroundColor = GetConsoleColor(color);
            System.Console.Write(message);
            System.Console.ResetColor();
        }

        private static ConsoleColor GetConsoleColor(Color color) =>
            color switch
            {
                Color.Blue => ConsoleColor.Blue,
                Color.Cyan => ConsoleColor.Cyan,
                Color.Green => ConsoleColor.Green,
                Color.Magenta => ConsoleColor.Magenta,
                Color.Yellow => ConsoleColor.Yellow,
                Color.None => ConsoleColor.White,
                _ => ConsoleColor.White
            };

        public static void OverwritePrintedChar(this string message, Color color = Color.None)
        {
            int currentLeft = System.Console.CursorLeft;
            int currentTop = System.Console.CursorTop;

            System.Console.SetCursorPosition(currentLeft - 1, currentTop);

            message.Write(color);
            System.Console.SetCursorPosition(currentLeft, currentTop);
        }

        public static void PrintGameMessage(this string message, bool bringBackCursor = true)
        {
            int currentLeft = System.Console.CursorLeft;
            int currentTop = System.Console.CursorTop;

            ClearLine();

            System.Console.SetCursorPosition(0, System.Console.BufferHeight);
            System.Console.Write(message);

            if (bringBackCursor)
            {
                System.Console.SetCursorPosition(currentLeft, currentTop);
            }

            void ClearLine()
            {
                System.Console.SetCursorPosition(0, System.Console.BufferHeight);
                System.Console.Write(new String(' ', System.Console.BufferWidth - 1));
            }
        }
    }
}


