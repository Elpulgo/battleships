using System;
using System.Collections.Generic;
using Core.Models;
using static Core.Models.CoordinatesHelper;

namespace AI_lib
{
    internal class MonteCarloWithHuntPrediciton : MonteCarloPrediciton
    {
        private static readonly Lazy<MonteCarloWithHuntPrediciton> lazy
        = new Lazy<MonteCarloWithHuntPrediciton>(() => new MonteCarloWithHuntPrediciton());

        public static new MonteCarloWithHuntPrediciton Instance { get { return lazy.Value; } }

        protected MonteCarloWithHuntPrediciton()
        {
        }

        public override (Column Column, int Row) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            base.BuildHits(currentGameBoardState);

            return base.IsInHuntMode ?
                base.PredictHunter(currentGameBoardState) :
                base.PredictBySimulation(currentGameBoardState);
        }
    }
}