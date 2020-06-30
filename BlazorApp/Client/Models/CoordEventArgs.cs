using System;
using static Core.Models.CoordinatesHelper;

namespace BlazorApp.Client.Models
{
    public class CoordEventArgs : EventArgs
    {
        public CoordEventArgs(Column column, int row)
        {
            Column = column;
            Row = row;
        }

        public Column Column { get; }
        public int Row { get; }
    }
}