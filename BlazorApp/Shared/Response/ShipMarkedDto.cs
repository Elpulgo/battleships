using Core.Models;

namespace Shared
{
    public class ShipMarkedDto
    {
        public ShipMarkedDto()
        {
        }

        public ShipMarkedDto(
           bool shipFound,
           bool shipDestroyed)
        {
            ShipFound = shipFound;
            ShipDestroyed = shipDestroyed;
        }

        public bool ShipFound { get; set; }

        public bool ShipDestroyed { get; set; }
    }
}
