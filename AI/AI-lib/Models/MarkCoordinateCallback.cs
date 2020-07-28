using System;
using System.Collections.Generic;
using System.Linq;

namespace AI_lib
{
    public class MarkCoordinateCallback
    {
        public MarkCoordinateCallback(
            bool shipFound,
            string key)
        {
            ShipFound = shipFound;
            Key = key;
            DestroyedShipCoordinates = new List<string>();
        }

        public MarkCoordinateCallback WithDestroyedShip(List<string> destroyedShipCoordinates)
        {
            if (destroyedShipCoordinates == null || !destroyedShipCoordinates.Any())
                throw new ArgumentNullException("Coordinates for destroyed ship can't be null or empty");

            ShipDestroyed = true;
            DestroyedShipCoordinates = destroyedShipCoordinates;
            return this;
        }

        public string Key { get; }
        public bool ShipFound { get; }
        public bool ShipDestroyed { get; private set; }

        public List<string> DestroyedShipCoordinates { get; private set; }
    }
}