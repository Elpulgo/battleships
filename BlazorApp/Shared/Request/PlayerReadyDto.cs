using System;
using System.Collections.Generic;
using Core.Models.Ships;

namespace Shared
{
    public class PlayerReadyDto
    {
        public PlayerReadyDto()
        {
        }
        public PlayerReadyDto(List<Ship> ships, Guid playerId)
        {
            Ships = ships;
            PlayerId = playerId;
        }

        public ICollection<Ship> Ships { get; set; }

        public Guid PlayerId { get; set; }
    }
}
