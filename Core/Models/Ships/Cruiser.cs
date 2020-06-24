using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models.Ships
{
    public class Cruiser : ShipBase
    {
        public Cruiser(IEnumerable<(Column, int)> coordinates)
            : base(coordinates)
        {
            ShipValidator.ValidateCoordinates<Cruiser>(this, coordinates.ToList());
        }

        public override string Name => this.GetType().Name;

        public override ShipType ShipType => ShipType.Cruiser;
    }
}

