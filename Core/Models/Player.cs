using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class Player
    {
        public string Name { get; }
        public PlayerType Type { get; }
        public int HitsOnOpponent { get; private set; }
        public int HitsOnPlayer { get; private set; }
        
        public Dictionary<ShipType, bool> OpponentShipStatus { get; set; }
        public Dictionary<ShipType, bool> PlayerShipStatus { get; set; }

        public Player(string name, PlayerType type)
        {
            Name = name;
            Type = type;
        }
    }

    public enum PlayerType
    {
        Computer,
        Human
    }
}
