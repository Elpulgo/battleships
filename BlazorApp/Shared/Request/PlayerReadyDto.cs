using System.Collections.Generic;
using Core.Models.Ships;

namespace Shared
{
    public class PlayerReadyDto
    {
        public ICollection<ShipBase> Ships { get; set; }

        public string Name { get; set; }
    }
}
