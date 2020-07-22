using System;
using System.Threading.Tasks;
using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace BlazorApp.Server.Services
{
    public interface IGamePlayRelay
    {
        Task<(bool ShipFound, bool ShipDestroyed)> MarkCoordinateAsync(
            Column column,
            int row,
            Guid playerId);

        GameBoardBase GetGameBoard(Guid playerId);
        GameBoardBase GetOpponentGameBoard(Guid playerId);
        (GameBoardBase board, GameBoardBase opponentBoard) GetFinalBoards(Guid playerId);
        void ResetGame();
    }
}