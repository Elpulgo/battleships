using System;
using System.Collections.Generic;
using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    /// <summary>
    /// Intended to be used as a singleton since it will hold
    /// information about the board to calculate predictions during the game
    /// </summary>
    public interface IAIManager
    {
        (Column Column, int Row, Action<MarkCoordinateCallback> resultFromMark) PredictCoordinate(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState);

        void Reset();
    }
}