using System;
using System.Collections.Concurrent;
using System.Linq;
using Core.Models;

namespace Core.Managers
{
    ///<summary>
    /// Intended to be used as a singleton.
    /// This instance holds the state of the game and the actions on the gameboards is invoked here.
    ///</summary>
    public interface IGameManager
    {
        /// <summary>
        /// Adds a board to the manager. Maximum number of boards per game is 2
        /// Takes <see cref="Core.Models.GameBoardBase"/>
        /// </summary>
        void AddBoard(GameBoardBase board);

        /// <summary>
        /// Get the board for specified player <paramref name="playerId"/>.
        /// </summary>
        /// <returns><see cref="Core.Models.GameBoardBase"/></returns>
        GameBoardBase GetGameBoard(Guid playerId);
        /// <summary>
        /// Get boards for all players, with all ships and marks visible.
        /// Note that <paramref name="playerId"/> should be current player 
        /// to get desired result.
        /// </summary>
        /// <returns><see cref="Core.Models.GameBoardBase"/></returns>
        (GameBoardBase board, GameBoardBase opponentBoard) GetAllBoards(Guid playerId);
        /// <summary>
        /// Get the board for opponent player. Note that <paramref name="playerId"/> should be current player 
        /// to get desired result.
        /// </summary>
        /// <returns><see cref="Core.Models.GameBoardBase"/></returns>
        GameBoardBase GetOpponentBoard(Guid playerId);

        /// <summary>
        /// Check if all ships on the board for player is destroyed <paramref name="playerId"/>.
        /// </summary>
        /// <returns>Indication if all ships for current player is destroyed.</returns>
        bool IsAllShipsDestroyedForPlayer(Guid playerId);

        /// <summary>
        /// Check if all ships on the board for the player opponent is destroyed.
        /// Note that the board asked for is the opponent board. Pass the playerid for the current user to get desired result.
        /// <paramref name="playerId"/>.
        /// </summary>
        /// <returns>Indication if all ships for opponent is destroyed.</returns>
        bool IsAllShipsDestroyedForOpponent(Guid playerId);

        /// <summary>
        /// Mark coordinate on opponent board.
        /// Note that the current user should be passed to get desired result.
        /// <paramref name="playerId"/>.
        /// <paramref name="coordinateKey"/>.
        /// </summary>
        /// <remarks>
        /// Coordinate key should be passed in the format 'B7' ({column}{row}), ranging from column A - J and row 1 - 10.
        /// </remarks>
        /// <returns>Indication if a ship was found on the coordinate, and if so, the ship has been destroyed.</returns>
        (bool shipFound, bool shipDestroyed) MarkCoordinate(Guid playerId, string coordinateKey);

        bool IsAllBoardsSetup { get; }
    }

    ///<summary>
    /// Intended to be used as a singleton.
    /// This instance holds the state of the game and the actions on the gameboards is invoked here.
    ///</summary>
    public class GameManager : IGameManager
    {
        private const int MaxNumberOfBoards = 2;
        private ConcurrentDictionary<Guid, GameBoardBase> _gameBoardLookup;

        private object m_Lock = new object();
        public bool IsAllBoardsSetup => _gameBoardLookup.Count == MaxNumberOfBoards;

        public GameManager()
        {
            _gameBoardLookup = new ConcurrentDictionary<Guid, GameBoardBase>();
        }

        public void AddBoard(GameBoardBase board)
        {
            if (_gameBoardLookup.Count == MaxNumberOfBoards)
                throw new Exception("Only 2 boards are allowed per game!");

            if (!_gameBoardLookup.TryAdd(board.Player.Id, board))
            {
                throw new Exception($"Failed to add gameboard for player with id '{board.Player.Id}'!");
            }
        }

        public GameBoardBase GetGameBoard(Guid playerId)
            => FindBoard(playerId);

        public GameBoardBase GetOpponentBoard(Guid playerId)
            => FindBoard(_gameBoardLookup.SingleOrDefault(f => f.Key != playerId).Key).ForOpponent();

        public bool IsAllShipsDestroyedForOpponent(Guid playerId)
            => FindBoard(_gameBoardLookup.SingleOrDefault(f => f.Key != playerId).Key).IsAllDestroyed();

        public bool IsAllShipsDestroyedForPlayer(Guid playerId)
            => FindBoard(playerId).IsAllDestroyed();

        public (bool shipFound, bool shipDestroyed) MarkCoordinate(Guid playerId, string coordinateKey)
            => FindBoard(_gameBoardLookup.SingleOrDefault(f => f.Key != playerId).Key).MarkCoordinate(coordinateKey);

        private GameBoardBase FindBoard(Guid playerId)
        {
            lock (m_Lock)
            {
                if (_gameBoardLookup.TryGetValue(playerId, out var gameBoard))
                {
                    return gameBoard;
                }
            }

            throw new Exception($"Player with id '{playerId}' doesn't exist in this game!");
        }

        public (GameBoardBase board, GameBoardBase opponentBoard) GetAllBoards(Guid playerId)
        {
            var board = FindBoard(playerId);
            var opponentBoard = FindBoard(_gameBoardLookup.SingleOrDefault(f => f.Key != playerId).Key);
            return (board, opponentBoard);
        }
    }
}
