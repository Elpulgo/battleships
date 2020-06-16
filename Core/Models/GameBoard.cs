using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class GameBoard
    {
        private Dictionary<string, CoordinateContainer> _matrix;
        public GameBoard()
        {
            _matrix = new Dictionary<string, CoordinateContainer>();
        }

        public void BuildMatrix(List<ShipContainer> ships)
        {
            _matrix = ships
                .SelectMany(ship => ship.Coordinates)
                .ToDictionary(coord => coord.Key);

            foreach (Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
            {
                foreach (var row in Enumerable.Range(1, CoordinatesHelper.GetRowCount()))
                {
                    var key = $"{column.ToString()}{row}";
                    _matrix.TryAdd(key, new CoordinateContainer(column, row));
                }
            }
        }
    }
}
