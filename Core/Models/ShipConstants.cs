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

        public static int NrOfBoxes(this ShipType type) =>
            type switch
            {
                ShipType.AirCraft => 5,
                ShipType.BattleShip => 4,
                ShipType.Cruiser => 3,
                ShipType.Destroyer => 2,
                ShipType.SubMarine => 2,
                _ => 0
            };
    }

    public enum ShipType
    {
        SubMarine,
        Destroyer,
        Cruiser,
        BattleShip,
        AirCraft
    }
}

