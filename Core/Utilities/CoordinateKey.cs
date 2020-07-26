using System;
using static Core.Models.CoordinatesHelper;

namespace Core.Utilities
{
    public static class CoordinateKey
    {
        public static string Build(Column column, int row)
            => $"{column}{row}";
        public static string Build(int column, int row)
            => $"{(Column)column}{row}";

        public static (Column Column, int Row) Parse(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("Key can't be null or empty when parsing!");

            var stringifiedColumn = key.Substring(0, 1);
            var stringifiedRow = key.Substring(1);

            if (!Enum.TryParse(typeof(Column), stringifiedColumn, true, out var column))
                throw new ArgumentException("Failed to parse column from string!");

            if (!int.TryParse(stringifiedRow, out var row))
                throw new ArgumentException("Failed to parse row from string!");

            return ((Column)column, row);
        }
    }
}

