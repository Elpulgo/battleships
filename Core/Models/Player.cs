using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public class Player
    {
        public string Name { get; }
        public PlayerType Type { get; }

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
