using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Utilities;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    /// <summary>
    /// Intended to be used as a singleton since it will hold
    /// information about the board to calculate predictions during the game
    /// </summary>
    public class AIManager : IAIManager
    {
        private readonly AILevel _level;
        private Random _random;
        private RandomPrediction _randomPrediction;
        private HunterPrediction _hunterPrediction;
        private MonteCarloPrediciton _monteCarloPrediction;

        public AIManager(AILevel level)
        {
            _level = level;
            _random = new Random();
            InitializePredictors();
        }

        public (Column Column, int Row, Action<MarkCoordinateCallback> resultFromMark) PredictCoordinate(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
            => _level switch
            {
                AILevel.Random => _randomPrediction.Predict(currentGameBoardState),
                AILevel.Hunter => _hunterPrediction.Predict(currentGameBoardState),
                AILevel.MonteCarloAndHunt => _monteCarloPrediction.Predict(currentGameBoardState),
                _ => throw new ArgumentException("A level for the AI has not been set, can't do any prediction!")
            };

        public void Reset()
        {
            // TODO: Reset all IPredictions...
        }

        private void InitializePredictors()
        {
            _randomPrediction = new RandomPrediction();
            _hunterPrediction = new HunterPrediction();
            _monteCarloPrediction = new MonteCarloPrediciton();
        }
    }
}