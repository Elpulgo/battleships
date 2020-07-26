using System.Collections.Generic;
using System.Linq;

namespace AI_lib
{
    internal class Neighbours
    {
        public Neighbours()
        {

        }

        public string NeighbourUp { get; set; }
        public string NeighbourDown { get; set; }
        public string NeighbourLeft { get; set; }
        public string NeighbourRight { get; set; }

        public List<string> AvailableNeighbours => new List<string>()
            {
                NeighbourUp,
                NeighbourDown,
                NeighbourLeft,
                NeighbourRight
            }
            .Where(w => !string.IsNullOrEmpty(w))
            .ToList();
    }
}