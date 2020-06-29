using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SetupController : ControllerBase
    {
        public SetupController()
        {

        }

        [HttpPost]
        public IActionResult AddPlayer()
        {
            // Body with name and gamemode(human or computer)
            // Return Core.Model.Player with id as Guid, connected to signalR guid id?
            // Exception/Error model if 2 players already exist
            return null;

        }

        [HttpPost]
        public IActionResult MarkShips()
        {
            // Body with list of ships? and player
            // Return succes or false, depending on validation?
            return null;

        }
    }
}