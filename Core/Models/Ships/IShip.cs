using System.Collections.Generic;

namespace Core.Models.Ships
{
    public interface IShip
    {
        ICollection<CoordinateContainer> Coordinates { get; }
        bool IsDestroyed { get; }

        int Boxes { get; }

        string Name { get; }

        Color Color { get; }

        bool HasCoordinate(string key);

        void MarkCoordinate(string key);
    }
}

