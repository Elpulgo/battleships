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
        private List<Ship> _ships;
        public Player Player { get; }
        public Dictionary<string, CoordinateContainer> Matrix { get; set; }

        public GameBoard()
        {

        }

        public GameBoard(Player player)
        {
            Matrix = new Dictionary<string, CoordinateContainer>();
            _ships = new List<Ship>();
            Player = player;
        }

        public GameBoard WithShips(List<Ship> ships)
        {
            _ships = ships;

            Matrix = ships
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
            Matrix[key].Mark();
            var ship = _ships.SingleOrDefault(ship => ship.HasCoordinate(key));
            if (ship == null)
                return (false, false);

            ship.MarkCoordinate(key);
            return (true, ship.IsDestroyed);
        }

        private void FillMatrix()
        {
            foreach (Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
            {
                foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
                {
                    var key = CoordinateKey.Build(column, row);
                    Matrix.TryAdd(key, new CoordinateContainer(column, row));
                }
            }
        }
    }
}
