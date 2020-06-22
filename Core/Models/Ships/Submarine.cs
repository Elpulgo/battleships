using System.Collections.Generic;
using System.Linq;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace Core.Models.Ships
{
    public class Submarine : ShipBase
    {
        public Submarine(IEnumerable<(Column, int)> coordinates)
        : base(coordinates)
        {
            ShipValidator.ValidateCoordinates<Submarine>(this, coordinates.ToList());
        }

        public override string Name => this.GetType().Name;

        public override Color Color => Color.Magenta;

        public override ShipType ShipType => ShipType.SubMarine;
    }
}

