using System;
using System.Collections.Generic;
using Core.Models.Ships;
using static Core.Models.CoordinatesHelper;

namespace Core.Factories
{
    public class ShipFactory
    {
        public ShipFactory()
        {

        }

        /// <param name="shipType">What type of ship to build.</param>
        /// <param name="coords">Coordinates for ship, Column, (A,B..), row (1,2..).</param>
        /// <param name="shipType">What type of ship to build.</param>
        /// <returns>Return an IShip.</returns>
        /// <exception cref="Core.Factories.ShipBuildException">Thrown when ShipType is not supported</exception>
        public IShip Build(
            ShipType shipType,
            IEnumerable<(Column, int)> coords) =>
            shipType switch
            {
                ShipType.AirCraft => new AircraftCarrier(coords),
                ShipType.BattleShip => new Battleship(coords),
                ShipType.Cruiser => new Cruiser(coords),
                ShipType.Destroyer => new Destroyer(coords),
                ShipType.SubMarine => new Submarine(coords),
                _ => throw new ShipBuildException($"{shipType.ToString()} is not supported.")
            };
    }

    public class ShipBuildException : Exception
    {
        public ShipBuildException(string message) : base(message)
        {

        }
    }
}

