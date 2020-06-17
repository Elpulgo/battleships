using static Core.Models.CoordinatesHelper;

namespace Core.Models
{
    public class CoordinateContainer
    {
        private int Row { get; }

        private Column Column { get; }

        public bool HasShip { get; private set; }

        public bool IsMarked { get; private set; }

        public string Key => $"{Column.ToString()}{Row}";

        public CoordinateContainer(Column column, int row)
        {
            Row = row;
            Column = column;
        }

        public CoordinateContainer WithShip()
        {
            HasShip = true;
            return this;
        }

        public void WasMarked()
        {
            IsMarked = true;
        }
    }
}
