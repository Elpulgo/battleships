
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace BlazorApp.Server.Managers
{
    public class PlayerManager
    {
        public bool IsPlayingVsComputer { get; private set; }
        public int PlayerCount => _players.Count;

        private List<Player> _players;

        public PlayerManager()
        {
            _players = new List<Player>();
        }

        public void PlayVsComputer() => IsPlayingVsComputer = true;

        public void AddPlayerToGame(Player player) => _players.Add(player);

        public Player GetPlayerById(Guid id) => _players.SingleOrDefault(s => s.Id == id);

    }
}