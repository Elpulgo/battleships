using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal sealed class CoordinateNeighbours
    {
        private static readonly Lazy<CoordinateNeighbours> lazy
            = new Lazy<CoordinateNeighbours>(() => new CoordinateNeighbours());

        public static CoordinateNeighbours Instance { get { return lazy.Value; } }

        private Dictionary<string, Neighbours> _neighbourMap;

        private CoordinateNeighbours()
        {
            BuildNeighbours();
        }

        public Neighbours GetNeighbours(string key)
        {
            if (_neighbourMap.TryGetValue(key, out var neighbours))
            {
                return neighbours;
            }

            return null;
        }

        private void BuildNeighbours()
        {
            _neighbourMap = new Dictionary<string, Neighbours>();

            foreach (Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
            {
                foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
                {
                    var key = CoordinateKey.Build(column, row);

                    var neighbourUp = GetNeighbourUp(column, row);
                    var neighbourDown = GetNeighbourDown(column, row);
                    var neighbourLeft = GetNeighbourLeft(column, row);
                    var neighbourRight = GetNeighbourRight(column, row);

                    var neighbours = new Neighbours()
                    {
                        NeighbourUp = neighbourUp.Exist ? neighbourUp.Key : string.Empty,
                        NeighbourDown = neighbourDown.Exist ? neighbourDown.Key : string.Empty,
                        NeighbourLeft = neighbourLeft.Exist ? neighbourLeft.Key : string.Empty,
                        NeighbourRight = neighbourRight.Exist ? neighbourRight.Key : string.Empty,
                    };

                    _neighbourMap.Add(key, neighbours);
                }
            }
        }

        private (bool Exist, string Key) GetNeighbourUp(Column col, int row)
        {
            if (row == 1)
                return (false, string.Empty);

            var newRow = row - 1;
            var key = CoordinateKey.Build(col, newRow);
            return (true, key);
        }

        private (bool Exist, string Key) GetNeighbourDown(Column col, int row)
        {
            if (row == 10)
                return (false, string.Empty);

            var newRow = row + 1;
            var key = CoordinateKey.Build(col, newRow);
            return (true, key);
        }

        private (bool Exist, string Key) GetNeighbourLeft(Column col, int row)
        {
            if ((int)col == 1)
                return (false, string.Empty);

            var newCol = (Column)((int)col - 1);
            var key = CoordinateKey.Build(newCol, row);
            return (true, key);
        }

        private (bool Exist, string Key) GetNeighbourRight(Column col, int row)
        {
            if ((int)col == 10)
                return (false, string.Empty);

            var newCol = (Column)((int)col + 1);
            var key = CoordinateKey.Build(newCol, row);
            return (true, key);
        }
    }
}