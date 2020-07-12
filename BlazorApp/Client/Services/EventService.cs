using System;
using Core.Models;

namespace BlazorApp.Client.Services
{
    public interface IEventService
    {
        event EventHandler<GameMode> GameModeEventChanged;
        event EventHandler<Player> PlayerCreatedEventChanged;
        event EventHandler<GameBoardBase> GameBoardEventChanged;
        event EventHandler<GameBoardBase> OpponentGameBoardEventChanged;
        event EventHandler ReloadGameBoardEventChanged;
        event EventHandler ReloadOpponentGameBoardEventChanged;

        void GameModeChanged(GameMode gameMode);
        void PlayerCreated(Player player);
        void GameBoardChanged(GameBoardBase gameBoard);
        void OpponentGameBoardChanged(GameBoardBase gameBoard);
        void ReloadGameBoard();
        void ReloadOpponentGameBoard();
    }

    // Should handle events, other services should invoke events here, which are 
    // subscribed to in index.razor and then used as CascadingValues, to have a single
    // point of subscription for data in the app
    public class EventService : IEventService
    {
        public event EventHandler<GameMode> GameModeEventChanged;
        public event EventHandler<Player> PlayerCreatedEventChanged;
        public event EventHandler<GameBoardBase> GameBoardEventChanged;
        public event EventHandler<GameBoardBase> OpponentGameBoardEventChanged;
        public event EventHandler ReloadGameBoardEventChanged;
        public event EventHandler ReloadOpponentGameBoardEventChanged;

        public EventService()
        {

        }

        public void PlayerCreated(Player player) => PlayerCreatedEventChanged?.Invoke(this, player);

        public void GameModeChanged(GameMode gameMode) => GameModeEventChanged?.Invoke(this, gameMode);

        public void GameBoardChanged(GameBoardBase gameBoard) => GameBoardEventChanged?.Invoke(this, gameBoard);
        public void OpponentGameBoardChanged(GameBoardBase gameBoard) => OpponentGameBoardEventChanged?.Invoke(this, gameBoard);
        public void ReloadGameBoard() => ReloadGameBoardEventChanged?.Invoke(this, new EventArgs());
        public void ReloadOpponentGameBoard() => ReloadOpponentGameBoardEventChanged?.Invoke(this, new EventArgs());
    }
}