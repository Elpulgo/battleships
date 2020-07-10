using Core.Models.Ships;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models
{
    public class CoordinateContainer
    {
        public int Row { get; set; }

        public Column Column { get; set;}

        public bool HasShip { get; set; }

        public bool IsMarked { get; set; }

        public Color Color { get; set; }

        public string Key => CoordinateKey.Build(Column, Row);

        public CoordinateContainer()
        {

        }
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

        public CoordinateContainer WithColor(Color color)
        {
            Color = color;
            return this;
        }

        public void Mark()
        {
            IsMarked = true;
        }
    }
}
