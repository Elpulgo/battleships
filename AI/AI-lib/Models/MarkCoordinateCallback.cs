using System.Collections.Generic;
using System.Linq;

namespace AI_lib
{
    public class MarkCoordinateCallback
    {
        public MarkCoordinateCallback(
            bool shipFound,
            bool shipDestroyed,
            string key)
        {
            ShipFound = shipFound;
            ShipDestroyed = shipDestroyed;
            Key = key;
        }

        public string Key { get; }
        public bool ShipFound { get; }
        public bool ShipDestroyed { get; }
    }
}