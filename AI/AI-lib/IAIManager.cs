using System;
using System.Collections.Generic;
using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    public interface IAIManager
    {
        (Column Column, int Row, Action<MarkCoordinateCallback> resultFromMark) PredictCoordinate(
            AILevel level,
            Dictionary<string, CoordinateContainerBase> currentGameBoardState);
    }
}