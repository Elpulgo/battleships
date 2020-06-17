using System;

namespace Core.Models
{
    public static class CoordinatesHelper
    {
        public static int GetRowCount() => 10;
        public static int GetColumnCount() => 10;
        public enum Column
        {
            A,
            B,
            C,
            D,
            E,
            F,
            G,
            H,
            I,
            J
        }

        public class CoordinateException : Exception
        {
            public CoordinateException(string message) : base(message)
            {

            }
        }
    }
}
