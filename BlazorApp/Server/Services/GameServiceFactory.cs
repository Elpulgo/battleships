using BlazorApp.Server.Managers;

namespace BlazorApp.Server.Services
{

    public class GameServiceFactory
    {

        public GameServiceFactory()
        {
        }

        public IGameService Build()
        {
            return new HumanoidGameService();
        }

    }
}