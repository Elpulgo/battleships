using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models
{
    public class CoordinateContainer
    {
        public int Row { get; }

        public Column Column { get; }

        public bool HasShip { get; private set; }

        public bool IsMarked { get; private set; }

        public string Key => CoordinateKey.Build(Column, Row);

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

        public void Mark()
        {
            IsMarked = true;
        }
    }
}
