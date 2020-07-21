using System;
using Core.Models;

namespace BlazorApp.Client.Services
{
    public class EventService : IEventListener, IEventExecutor
    {
        public event EventHandler<GameMode> GameModeEventChanged;
        public event EventHandler<Player> PlayerCreatedEventChanged;
        public event EventHandler<GameBoardBase> GameBoardEventChanged;
        public event EventHandler<GameBoardBase> OpponentGameBoardEventChanged;
        public event EventHandler ReloadGameBoardEventChanged;
        public event EventHandler ReloadOpponentGameBoardEventChanged;
        public event EventHandler<bool> PlayerTurnEventChanged;
        public event EventHandler<(bool shipWasHit, bool shipWasDestroyed)> OpponentMoveFiredEventChanged;
        public event EventHandler<string> WinnerNominatedEventChanged;

        public EventService()
        {
        }

        public void PlayerCreated(Player player)
            => PlayerCreatedEventChanged?.Invoke(this, player);
        public void GameModeChanged(GameMode gameMode)
            => GameModeEventChanged?.Invoke(this, gameMode);
        public void GameBoardChanged(GameBoardBase gameBoard)
            => GameBoardEventChanged?.Invoke(this, gameBoard);
        public void OpponentGameBoardChanged(GameBoardBase gameBoard)
            => OpponentGameBoardEventChanged?.Invoke(this, gameBoard);
        public void ReloadGameBoard()
            => ReloadGameBoardEventChanged?.Invoke(this, new EventArgs());
        public void ReloadOpponentGameBoard()
            => ReloadOpponentGameBoardEventChanged?.Invoke(this, new EventArgs());
        public void PlayerTurnChanged(bool isPlayerTurn)
            => PlayerTurnEventChanged?.Invoke(this, isPlayerTurn);

        public void OpponentMoveFired(bool shipWasHit, bool shipWasDestroyed)
            => OpponentMoveFiredEventChanged?.Invoke(this, (shipWasHit, shipWasDestroyed));

        public void WinnerNominated(string winner)
            => WinnerNominatedEventChanged?.Invoke(this, winner);
    }
}