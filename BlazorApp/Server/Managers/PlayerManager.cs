
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace BlazorApp.Server.Managers
{
    public class PlayerManager
    {
        public bool IsPlayingVsComputer { get; private set; }
        public List<Player> Players { get; private set; }
        public int PlayerCount => Players.Count;

        public PlayerManager()
        {
            Players = new List<Player>();
        }

        public void PlayVsComputer() => IsPlayingVsComputer = true;

        public void AddPlayerToGame(Player player) => Players.Add(player);

        public Player GetPlayerById(Guid id) => Players.SingleOrDefault(s => s.Id == id);

        public Player GetOpponent(Guid id) => Players.SingleOrDefault(s => s.Id != id);

        public void Reset()
        {
            Players.Clear();
            IsPlayingVsComputer = false;
        }
    }
}