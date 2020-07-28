using System;
using System.Collections.Generic;
using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal abstract class PredictionBase
    {

        public PredictionBase()
        {
        }

        public abstract (Column Column, int Row, Action<MarkCoordinateCallback> callback) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState);
    }
}