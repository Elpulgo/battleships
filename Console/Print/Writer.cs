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
    }
}


