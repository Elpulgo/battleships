using System;
using Core.Models;

namespace BlazorApp.Client.Services
{

    public interface IEventService
    {
        event EventHandler<GameMode> GameModeEventChanged;
        void GameModeChanged(GameMode gameMode);
    }

    // Should handle events, other services should invoke events here, which are 
    // subscribed to in index.razor and then used as CascadingValues, to have a single
    // point of subscription for data in the app
    public class EventService : IEventService
    {
        public event EventHandler<GameMode> GameModeEventChanged;
        public EventService()
        {

        }

        public void GameModeChanged(GameMode gameMode) => GameModeEventChanged.Invoke(this, gameMode);
    }
}