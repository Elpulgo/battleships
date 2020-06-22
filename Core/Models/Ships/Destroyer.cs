using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models.Ships
{
    public class Destroyer : ShipBase
    {
        public Destroyer(IEnumerable<(Column, int)> coordinates)
        : base(coordinates)
        {
            ShipValidator.ValidateCoordinates<Destroyer>(this, coordinates.ToList());
        }

        public override string Name => this.GetType().Name;

        public override Color Color => Color.Green;

        public override ShipType ShipType => ShipType.Destroyer;

    }
}

