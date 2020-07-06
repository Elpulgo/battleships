using System;
using System.Collections.Generic;
using Core.Models.Ships;
using static Core.Models.CoordinatesHelper;

namespace BlazorApp.Client.Models
{
    public class AllShipsMarkedEventArgs : EventArgs
    {
        public List<Ship> Ships { get; }
        
        public AllShipsMarkedEventArgs(List<Ship> ships)
        {
            Ships = ships;
        }

    }
}