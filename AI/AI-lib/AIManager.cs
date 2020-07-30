using System;
using System.Collections.Generic;
using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    public class AIManager : IAIManager
    {
        public AIManager()
        {
        }

        public (Column Column, int Row, Action<MarkCoordinateCallback> resultFromMark) PredictCoordinate(
            AILevel level,
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
            => level switch
            {
                AILevel.Random => RandomPrediction.Instance.Predict(currentGameBoardState),
                AILevel.Hunter => HunterPrediction.Instance.Predict(currentGameBoardState),
                AILevel.MonteCarlo => MonteCarloPrediciton.Instance.Predict(currentGameBoardState),
                AILevel.MonteCarloAndHunt => MonteCarloWithHuntPrediciton.Instance.Predict(currentGameBoardState),
                _ => throw new ArgumentException("A level for the AI has not been set, can't do any prediction!")
            };
    }
}