using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Server.Managers;
using Core.Managers;
using Core.Models;
using Core.Models.Ships;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace BlazorApp.Server.Services
{
    public class GameActionRelay : ISetupRelay, IGamePlayRelay
    {
        private readonly GameServiceFactory _gameServiceFactory;
        private readonly ConnectionManager _connectionManager;
        private readonly IPushNotificationService _pushNotificationService;
        private readonly IGameManager _gameManager;
        private readonly PlayerManager _playerManager;
        private List<Guid> _finalBoardRequests;

        public GameActionRelay(
            GameServiceFactory gameServiceFactory,
            ConnectionManager connectionManager,
            IPushNotificationService pushNotificationService,
            IGameManager gameManager,
            PlayerManager playerManager)
        {
            _finalBoardRequests = new List<Guid>();
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

            if (_playerManager.IsPlayingVsComputer)
            {
                await PlayerReadyVsComputerAsync(playerId);
                return;
            }

            if (!_gameManager.IsAllBoardsSetup)
            {
                await _pushNotificationService.GameModeChangedClientAsync(GameMode.WaitingForPlayerSetup, playerId);
                return;
            }

            await _pushNotificationService.GameModeChangedAllAsync(GameMode.GamePlay);

            var randomPlayer = GetRandomPlayer();
            var opponent = _playerManager.GetOpponent(randomPlayer.Id);
            await _pushNotificationService.PlayerTurnChangedAsync(randomPlayer.Id);
            await _pushNotificationService.PlayerWaitChangedAsync(opponent.Id);
        }

        private async Task PlayerReadyVsComputerAsync(Guid playerId)
        {
            var computer = _playerManager.GetOpponent(playerId);
            var computerShips = new ShipGenerator().Generate().ToList();

            _gameManager.AddBoard(new GameBoardBase(computer).WithShips(computerShips));

            await _pushNotificationService.GameModeChangedAllAsync(GameMode.GamePlay);

            var randomPlayer = GetRandomPlayer();

            // Fire to computer service here aswell..
            if (randomPlayer.Id == playerId)
            {
                await _pushNotificationService.PlayerTurnChangedAsync(randomPlayer.Id);
            }
            else
            {
                await _pushNotificationService.PlayerWaitChangedAsync(randomPlayer.Id);
            }
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

        #region Gameplay


        public GameBoardBase GetGameBoard(Guid playerId)
            => _gameManager.GetGameBoard(playerId);

        public GameBoardBase GetOpponentGameBoard(Guid playerId)
            => _gameManager.GetOpponentBoard(playerId);

        public (GameBoardBase board, GameBoardBase opponentBoard) GetFinalBoards(Guid playerId)
        {
            _finalBoardRequests.Add(playerId);

            var (board, opponentBoard) = _gameManager.GetAllBoards(playerId);

            if (_playerManager.Players.All(s => _finalBoardRequests.Contains(s.Id)) || _playerManager.IsPlayingVsComputer)
            {
                ResetGame();
            }

            return (board, opponentBoard);
        }

        public async Task<(bool ShipFound, bool ShipDestroyed)> MarkCoordinateAsync(
            CoordinatesHelper.Column column,
            int row,
            Guid playerId)
        {
            // Need special handling when playing vs computer
            // This method need to be called from computer

            if (IsPlayerComputer(playerId))
            {
                return await HandleMarkCoordinateAsComputerAsync(playerId, column, row);
            }

            var (shipFound, shipDestroyed) = _gameManager.MarkCoordinate(
                playerId,
                CoordinateKey.Build(column, row));


            if (_gameManager.IsAllShipsDestroyedForOpponent(playerId))
            {
                await _pushNotificationService.GameEndedAllAsync(playerId);
                return (shipFound, shipDestroyed);
            }

            var opponentPlayer = _playerManager.GetOpponent(playerId);

            await _pushNotificationService.ReloadGameBoardAsync(opponentPlayer.Id, shipFound, shipDestroyed);
            await _pushNotificationService.ReloadOpponentGameBoardAsync(playerId);

            await _pushNotificationService.PlayerTurnChangedAsync(opponentPlayer.Id);
            await _pushNotificationService.PlayerWaitChangedAsync(playerId);

            return (shipFound, shipDestroyed);
        }

        public void ResetGame()
        {
            _gameManager.Reset();
            _playerManager.Reset();
            _connectionManager.Reset();
            _finalBoardRequests.Clear();
        }

        private bool IsPlayerComputer(Guid playerId)
        {
            if (!_playerManager.IsPlayingVsComputer)
                return false;

            if (_playerManager.GetPlayerById(playerId).Type == PlayerType.Computer)
                return true;

            return false;
        }

        private async Task<(bool ShipFound, bool ShipDestroyed)> HandleMarkCoordinateAsComputerAsync(
            Guid playerId,
            Column column,
            int row)
        {
            var (shipFound, shipDestroyed) = _gameManager.MarkCoordinate(
               playerId,
               CoordinateKey.Build(column, row));

            if (_gameManager.IsAllShipsDestroyedForOpponent(playerId))
            {
                await _pushNotificationService.GameEndedAllAsync(playerId);
                return (shipFound, shipDestroyed);
            }

            var humanPlayer = _playerManager.GetOpponent(playerId);
            await _pushNotificationService.ReloadGameBoardAsync(humanPlayer.Id, shipFound, shipDestroyed);
            await _pushNotificationService.PlayerTurnChangedAsync(humanPlayer.Id);

            return (shipFound, shipDestroyed);
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