using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GamePlayController : ControllerBase
    {
        public GamePlayController()
        {

        }

        [HttpPost]
        public IActionResult MarkCoordinate()
        {
            // Body with player id and coordinate to mark
            // Return hit/miss/ship sunken?
            return null;
        }

        [HttpPost]
        public IActionResult PlayerReady()
        {
            // Send SignalR that player is ready?
            return null;

        }


    }
}