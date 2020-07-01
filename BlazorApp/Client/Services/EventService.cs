namespace BlazorApp.Client.Services
{

    // Should handle events, other services should invoke events here, which are 
    // subscribed to in index.razor and then used as CascadingValues, to have a single
    // point of subscription for data in the app
    public class EventService
    {
        public EventService()
        {

        }
    }
}