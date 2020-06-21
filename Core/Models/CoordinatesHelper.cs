using System;

namespace Core.Models
{
    public static class CoordinatesHelper
    {
        public enum Column
        {
            A = 1,
            B = 2,
            C = 3,
            D = 4,
            E = 5,
            F = 6,
            G = 7,
            H = 8,
            I = 9,
            J = 10
        }

        public class CoordinateException : Exception
        {
            public CoordinateException(string message) : base(message)
            {

            }
        }
    }
}
