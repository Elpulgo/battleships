using System;
using Core.Models.Ships;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models
{
    [Serializable]
    public class CoordinateContainerBase
    {
        public int Row { get; set; }

        public Column Column { get; set; }

        public bool HasShip { get; set; }

        public bool IsMarked { get; set; }
        public bool IsShipDestroyed => HasShip && IsMarked;

        public Color Color { get; set; }

        public string Key => CoordinateKey.Build(Column, Row);

        public CoordinateContainerBase()
        {

        }
        public CoordinateContainerBase(Column column, int row)
        {
            Row = row;
            Column = column;
        }

        public CoordinateContainerBase WithShip()
        {
            HasShip = true;
            return this;
        }

        public CoordinateContainerBase WithColor(Color color)
        {
            Color = color;
            return this;
        }

        /// <summary>
        /// Will clone the Coordinate so the original object is not tampered with since
        /// the state of the coordinate should be intact.
        /// </summary>
        public CoordinateContainerBase ForOpponent()
        {
            if (HasShip && IsMarked)
                return this;

            var clone = this.DeepClone();
            clone.HasShip = false;
            return clone;
        }

        public void Mark()
        {
            IsMarked = true;
        }
    }
}
