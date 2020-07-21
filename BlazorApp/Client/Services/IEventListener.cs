using System;
using Core.Models;

namespace BlazorApp.Client.Services
{
    public interface IEventListener
    {
        event EventHandler<GameMode> GameModeEventChanged;
        event EventHandler<Player> PlayerCreatedEventChanged;
        event EventHandler<GameBoardBase> GameBoardEventChanged;
        event EventHandler<GameBoardBase> OpponentGameBoardEventChanged;
        event EventHandler ReloadGameBoardEventChanged;
        event EventHandler ReloadOpponentGameBoardEventChanged;
        event EventHandler<bool> PlayerTurnEventChanged;
        event EventHandler<(bool shipWasHit, bool shipWasDestroyed)> OpponentMoveFiredEventChanged;
        event EventHandler<string> WinnerNominatedEventChanged;

    }
}