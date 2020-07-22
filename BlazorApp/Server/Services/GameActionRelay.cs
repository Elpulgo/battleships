using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Server.Managers;
using Core.Managers;
using Core.Models;
using Core.Models.Ships;

namespace BlazorApp.Server.Services
{
    public class GameActionRelay : ISetupRelay, IGamePlayRelay
    {
        private readonly GameServiceFactory _gameServiceFactory;
        private readonly ConnectionManager _connectionManager;
        private readonly PushNotificationService _pushNotificationService;
        private readonly GameManager _gameManager;
        private readonly PlayerManager _playerManager;

        public GameActionRelay(
            GameServiceFactory gameServiceFactory,
            ConnectionManager connectionManager,
            PushNotificationService pushNotificationService,
            GameManager gameManager,
            PlayerManager playerManager)
        {
            _gameServiceFactory = gameServiceFactory;
            _connectionManager = connectionManager;
            _pushNotificationService = pushNotificationService;
            _gameManager = gameManager;
            _playerManager = playerManager;
        }

        #region Setup

        public bool IsOtherPlayerCreated()
            => _playerManager.PlayerCount > 0 && !_playerManager.IsPlayingVsComputer;

        public bool IsPlayerSlotAvailable()
            => (!_playerManager.IsPlayingVsComputer && _connectionManager.Count < 2);

        public async Task<Player> CreatePlayer(
            string name,
            PlayerType type,
            bool playVsComputer,
            string connectionId)
        {
            if (_connectionManager.Count > 1)
                throw new SetupException("Maximum number of players already in the game!");

            var player = new Player(name, type);
            _connectionManager.Add(player.Id, connectionId);
            _playerManager.AddPlayerToGame(player);

            switch (playVsComputer, _connectionManager.Count)
            {
                case (true, 1):
                    _playerManager.PlayVsComputer();
                    await _pushNotificationService.GameModeChangedClientAsync(GameMode.Setup, player.Id);
                    break;
                case (false, 2):
                    await _pushNotificationService.GameModeChangedAllAsync(GameMode.Setup);
                    break;
                default:
                    await _pushNotificationService.GameModeChangedClientAsync(GameMode.WaitingForPlayerToJoin, player.Id);
                    break;
            }

            return player;
        }


        public async Task PlayerIsReady(Guid playerId, List<Ship> ships)
        {
            var player = _playerManager.GetPlayerById(playerId);

            _gameManager.AddBoard(new GameBoardBase(player).WithShips(ships));

            if (!_gameManager.IsAllBoardsSetup)
            {
                await _pushNotificationService.GameModeChangedClientAsync(GameMode.WaitingForPlayerSetup, playerId);
                return;
            }

            await _pushNotificationService.GameModeChangedAllAsync(GameMode.GamePlay);

            var randomPlayer = GetRandomPlayer();

            await _pushNotificationService.PlayerTurnChangedAsync(randomPlayer.Id);
        }

        private Player GetRandomPlayer()
        {
            var random = new Random();
            var nextRandom = random.Next(0, 1);
            if (_playerManager.PlayerCount < 2)
                throw new ArgumentOutOfRangeException("All players have not joined the gamed yet!");

            return _playerManager.Players.Skip(nextRandom).FirstOrDefault();
        }

        #endregion
    }

    public class SetupException : Exception
    {
        public SetupException(string message) : base(message)
        {
        }
    }
}