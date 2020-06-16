using System.Collections.Generic;
using System.Linq;

namespace Core.Models
{
    public static class ShipConstants
    {
        public static IEnumerable<ShipType> GetShipTypesPerPlayer()
        {
            yield return ShipType.AirCraft;
            yield return ShipType.BattleShip;
            yield return ShipType.Cruiser;
            yield return ShipType.Destroyer;
            yield return ShipType.Destroyer;
            yield return ShipType.SubMarine;
            yield return ShipType.SubMarine;
        }
    }

    public enum ShipType
    {
        SubMarine = 1,
        Destroyer = 2,
        Cruiser = 3,
        BattleShip = 4,
        AirCraft = 5,
    }
}

