namespace BlazorApp.Client.Services
{

    public interface IEventService
    {

    }

    // Should handle events, other services should invoke events here, which are 
    // subscribed to in index.razor and then used as CascadingValues, to have a single
    // point of subscription for data in the app
    public class EventService : IEventService
    {
        public EventService()
        {

        }
    }
}