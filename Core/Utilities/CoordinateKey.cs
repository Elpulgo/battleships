using static Core.Models.CoordinatesHelper;

namespace Core.Utilities
{
    public static class CoordinateKey
    {
        public static string Build(Column column, int row)
            => $"{column}{row}";
        public static string Build(int column, int row)
            => $"{(Column)column}{row}";
    }
}

