using System.Collections.Generic;
using Core.Utilities;

namespace Core.Models.Ships
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
                ShipType.SubMarine => 2,
                ShipType.Destroyer => 2,
                ShipType.Cruiser => 3,
                ShipType.BattleShip => 4,
                ShipType.AirCraft => 5,
                _ => throw new ShipValidationException($"Shiptype {type.ToString()} is not supported.")
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

