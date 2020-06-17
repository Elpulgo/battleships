using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class PlayerStatus
    {
        public Player Player { get; }
        public int HitsOnOpponent { get; private set; }
        public int HitsOnPlayer { get; private set; }

        public Dictionary<ShipType, bool> OpponentShipStatus { get; set; }
        public Dictionary<ShipType, bool> PlayerShipStatus { get; set; }

        public bool AllOpponentsShipsSunken => OpponentShipStatus.All(status => status.Value == true);

        public PlayerStatus(Player player)
        {
            OpponentShipStatus = ShipConstants.GetShipTypesPerPlayer().ToDictionary(ship => ship, value => false);
            PlayerShipStatus = ShipConstants.GetShipTypesPerPlayer().ToDictionary(ship => ship, value => false);
            Player = player;
        }

        public void IncrementHitsOnOpponent()
        {
            HitsOnOpponent++;
        }

        public void IncrementHitsOnPlayer()
        {
            HitsOnPlayer++;
        }
        public void MarkOpponentShipAsSunken(ShipType type)
        {
            OpponentShipStatus[type] = true;
        }

        public void MarkPlayerShipAsSunken(ShipType type)
        {
            PlayerShipStatus[type] = true;
        }
    }
}
