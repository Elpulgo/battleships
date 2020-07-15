using System;
using Core.Models;

namespace BlazorApp.Client.Services
{
    public interface IEventExecutor
    {
        void GameModeChanged(GameMode gameMode);
        void PlayerCreated(Player player);
        void GameBoardChanged(GameBoardBase gameBoard);
        void OpponentGameBoardChanged(GameBoardBase gameBoard);
        void ReloadGameBoard();
        void ReloadOpponentGameBoard();
        void PlayerTurnChanged(bool isPlayerTurn);
    }
}