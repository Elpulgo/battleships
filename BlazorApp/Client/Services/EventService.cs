using System;
using Core.Models;

namespace BlazorApp.Client.Services
{
    public interface IEventService
    {
        event EventHandler<GameMode> GameModeEventChanged;
        event EventHandler<Player> PlayerCreatedEventChanged;
        event EventHandler<GameBoard> GameBoardEventChanged;
        void GameModeChanged(GameMode gameMode);
        void PlayerCreated(Player player);
        void GameBoardChanged(GameBoard gameBoard);
    }

    // Should handle events, other services should invoke events here, which are 
    // subscribed to in index.razor and then used as CascadingValues, to have a single
    // point of subscription for data in the app
    public class EventService : IEventService
    {
        public event EventHandler<GameMode> GameModeEventChanged;
        public event EventHandler<Player> PlayerCreatedEventChanged;
        public event EventHandler<GameBoard> GameBoardEventChanged;

        public EventService()
        {

        }

        public void PlayerCreated(Player player) => PlayerCreatedEventChanged?.Invoke(this, player);

        public void GameModeChanged(GameMode gameMode) => GameModeEventChanged?.Invoke(this, gameMode);

        public void GameBoardChanged(GameBoard gameBoard) => GameBoardEventChanged?.Invoke(this, gameBoard);
    }
}