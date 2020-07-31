using System.Collections.Generic;
using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal abstract class PredictionBase
    {

        public static PredictionBase Instance { get; }
        public PredictionBase()
        {
        }

        public abstract (Column Column, int Row) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState);
    }
}