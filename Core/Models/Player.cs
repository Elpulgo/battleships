using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    [Serializable]
    public class Player
    {
        public Player()
        {
        }

        public Player(string name, PlayerType type)
        {
            Id = Guid.NewGuid();
            Name = name;
            Type = type;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public PlayerType Type { get; set; }
    }

    public enum PlayerType
    {
        Computer,
        Human
    }
}
