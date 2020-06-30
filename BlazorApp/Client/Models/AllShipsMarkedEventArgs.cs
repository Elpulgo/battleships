using System;
using System.Collections.Generic;
using Core.Models.Ships;
using static Core.Models.CoordinatesHelper;

namespace BlazorApp.Client.Models
{
    public class AllShipsMarkedEventArgs : EventArgs
    {
        public List<IShip> Ships { get; }
        
        public AllShipsMarkedEventArgs(List<IShip> ships)
        {
            Ships = ships;
        }

    }
}