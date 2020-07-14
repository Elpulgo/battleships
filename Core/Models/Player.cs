using System;
using System.Collections.Generic;
using System.Linq;
using Core.Utilities;

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

        /// <summary>
        /// Will clone the Player and strip the Id so opponent can't cheat in the game
        /// and do requests to server with opponent playerid.
        /// </summary>
        public Player ForOpponent()
        {
            var clone = this.DeepClone();
            clone.Id = Guid.Empty;
            return clone;
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
