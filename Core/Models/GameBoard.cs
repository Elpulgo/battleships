using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models.Ships;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models
{
    public class GameBoard
    {
        private Dictionary<string, CoordinateContainer> _matrix;
        private List<IShip> _ships;
        public Player Player { get; }
        public GameBoard(Player player)
        {
            _matrix = new Dictionary<string, CoordinateContainer>();
            Player = player;
            FillMatrix();
        }

        public GameBoard WithShips(List<IShip> ships)
        {

            _ships = ships;

            _matrix = ships
                .SelectMany(ship => ship.Coordinates)
                .ToDictionary(coord => coord.Key);

            FillMatrix();

            return this;
        }

        public bool IsAllDestroyed()
        {
            return _ships.All(ship => ship.IsDestroyed);
        }

        public (bool shipFound, bool shipDestroyed) MarkCoordinate(string key)
        {
            _matrix[key].Mark();
            var match = _ships.SingleOrDefault(ship => ship.HasCoordinate(key));
            if (match == null)
                return (false, false);

            match.MarkCoordinate(key);
            return (true, match.IsDestroyed);
        }

        private void FillMatrix()
        {
            foreach (Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
            {
                foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
                {
                    var key = CoordinateKey.Build(column, row);
                    _matrix.TryAdd(key, new CoordinateContainer(column, row));
                }
            }
        }
    }
}
