using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models.Ships;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models
{
    [Serializable]
    public class GameBoardBase : ICloneable<GameBoardBase>
    {
        private List<Ship> _ships;
        public Player Player { get; set; }
        public Dictionary<string, CoordinateContainerBase> Matrix { get; set; }

        public GameBoardBase()
        {

        }

        public GameBoardBase(Player player)
        {
            Matrix = new Dictionary<string, CoordinateContainerBase>();
            _ships = new List<Ship>();
            Player = player;
        }

        public GameBoardBase WithShips(List<Ship> ships)
        {
            _ships = ships;

            Matrix = ships
                .SelectMany(ship => ship.Coordinates)
                .ToDictionary(coord => coord.Key);

            FillMatrix();
            return this;
        }

        /// <summary>
        /// Will clone the Board so the original object is not tampered with since
        /// the state of the board should be intact.
        /// </summary>
        public GameBoardBase ForOpponent()
        {
            var clone = this.DeepClone();

            clone.Matrix = Matrix
                  .Select(s => KeyValuePair.Create(s.Key, s.Value.ForOpponent()))
                  .ToDictionary(d => d.Key, value => value.Value);

            clone.Player = Player.ForOpponent();

            return clone;
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

            if (ship.IsDestroyed)
            {
                foreach (var coordKey in ship.Coordinates.Select(s => s.Key))
                {
                    Matrix[coordKey].ShipWasDestroyed();
                }
            }

            return (true, ship.IsDestroyed);
        }

        public List<string> GetCoordinatesForDestroyedShip(string key)
        {
            if (!Matrix[key].IsShipDestroyed)
                return new List<string>();

            var ship = _ships.SingleOrDefault(s => s.Coordinates.Select(c => c.Key).Contains(key));
            if (ship == null || !ship.IsDestroyed)
                return new List<string>();

            return ship.Coordinates.Select(s => s.Key).ToList();
        }

        private void FillMatrix()
        {
            foreach (Column column in Enum.GetValues(typeof(CoordinatesHelper.Column)))
            {
                foreach (var row in Enumerable.Range(1, GameConstants.MaxRowCount))
                {
                    var key = CoordinateKey.Build(column, row);
                    Matrix.TryAdd(key, new CoordinateContainerBase(column, row));
                }
            }
        }
    }
}
