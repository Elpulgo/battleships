using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models.Ships
{
    public class AircraftCarrier : ShipBase
    {
        public AircraftCarrier(IEnumerable<(Column, int)> coordinates)
            : base(coordinates)
        {
            ShipValidator.ValidateCoordinates<AircraftCarrier>(this, coordinates.ToList());
        }

        public override string Name => this.GetType().Name;

        public override ShipType ShipType => ShipType.AirCraft;
    }
}

