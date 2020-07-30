using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AI_lib;
using BlazorApp.Server.Managers;
using BlazorApp.Shared;
using Core.Managers;
using Core.Models;
using Core.Models.Ships;
using Core.Utilities;
using Shared;
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
        private readonly IAIManager _AIManager;
        private List<Guid> _finalBoardRequests;

        public GameActionRelay(
            GameServiceFactory gameServiceFactory,
            ConnectionManager connectionManager,
            IPushNotificationService pushNotificationService,
            IGameManager gameManager,
            PlayerManager playerManager,
            IAIManager AIManager)
        {
            _finalBoardRequests = new List<Guid>();
            _gameServiceFactory = gameServiceFactory;
            _connectionManager = connectionManager;
            _pushNotificationService = pushNotificationService;
            _gameManager = gameManager;
            _playerManager = playerManager;
            _AIManager = AIManager;
        }

        #region Setup

        public bool IsOtherPlayerCreated()
            => _playerManager.PlayerCount > 0 && !_playerManager.IsPlayingVsComputer;

        public bool IsPlayerSlotAvailable()
            => (!_playerManager.IsPlayingVsComputer && _connectionManager.Count < 2);

        public async Task<Player> CreatePlayer(
            string name,
            PlayerType type,
            string connectionId,
            bool playVsComputer,
            ComputerLevel computerLevel)
        {
            if (_connectionManager.Count > 1)
                throw new SetupException("Maximum number of players already in the game!");

            var player = new Player(name, type);
            _connectionManager.Add(player.Id, connectionId);
            _playerManager.AddPlayerToGame(player);

            switch (playVsComputer, _connectionManager.Count)
            {
                case (true, 1):
                    _playerManager.PlayVsComputer(computerLevel);
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

            if (randomPlayer.Id == playerId)
            {
                await _pushNotificationService.PlayerTurnChangedAsync(randomPlayer.Id);
            }
            else
            {
                await _pushNotificationService.PlayerWaitChangedAsync(randomPlayer.Id);
                ExecuteComputerMove();
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

        #region Computer

        private (bool ShipFound, bool ShipDestroyed) ExecuteComputerMove()
        {
            var computerPlayer = _playerManager.Players.Single(s => s.Type == PlayerType.Computer);
            var opponentGameBoard = _gameManager.GetOpponentBoard(computerPlayer.Id);

            var (column, row, action) = _AIManager.PredictCoordinate(
                ConvertToAILevel(_playerManager.ComputerLevel),
                opponentGameBoard.Matrix);

            var predictedKey = CoordinateKey.Build(column, row);

            var result = _gameManager.MarkCoordinate(computerPlayer.Id, predictedKey);

            var callback = new MarkCoordinateCallback(result.shipFound, predictedKey);

            if (result.shipDestroyed)
            {
                var coordsForDestroyedShip = _gameManager
                    .GetOpponentBoard(computerPlayer.Id)
                    .GetCoordinatesForDestroyedShip(predictedKey);
                callback.WithDestroyedShip(coordsForDestroyedShip);
            }

            action.Invoke(callback);
            return result;

            AILevel ConvertToAILevel(ComputerLevel level) => level switch
            {
                ComputerLevel.Childish => AILevel.Random,
                ComputerLevel.Easy => AILevel.Hunter,
                ComputerLevel.Hard => AILevel.MonteCarlo,
                ComputerLevel.Impossible => AILevel.MonteCarloAndHunt,
                _ => throw new ArgumentException("Can't play vs computer and have no level!")
            };
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
            var (shipFound, shipDestroyed) = _gameManager.MarkCoordinate(
                playerId,
                CoordinateKey.Build(column, row));


            if (_gameManager.IsAllShipsDestroyedForOpponent(playerId))
            {
                await _pushNotificationService.GameEndedAllAsync(playerId);
                return (shipFound, shipDestroyed);
            }

            var opponentPlayer = _playerManager.GetOpponent(playerId);

            if (_playerManager.IsPlayingVsComputer)
            {
                await HandleComputerMoveAsync(playerId, opponentPlayer.Id);
                return (shipFound, shipDestroyed);
            }

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

        private async Task HandleComputerMoveAsync(
            Guid playerId,
            Guid computerId)
        {
            await _pushNotificationService.ReloadOpponentGameBoardAsync(playerId);
            await _pushNotificationService.PlayerWaitChangedAsync(playerId);

            var result = ExecuteComputerMove();

            // Simulate computer thinking..
            await Task.Delay(3000);

            await _pushNotificationService.ReloadGameBoardAsync(playerId, result.ShipFound, result.ShipDestroyed);

            if (_gameManager.IsAllShipsDestroyedForOpponent(computerId))
            {
                await _pushNotificationService.GameEndedAllAsync(computerId);
                return;
            }

            await _pushNotificationService.PlayerTurnChangedAsync(playerId);
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