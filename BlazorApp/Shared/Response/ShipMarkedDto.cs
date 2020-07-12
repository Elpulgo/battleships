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

        public bool ShipFound { get; private set; }

        public bool ShipDestroyed { get; private set; }
    }
}
