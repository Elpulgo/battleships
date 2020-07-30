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

        public override (Column Column, int Row, Action<MarkCoordinateCallback> callback) Predict(
            Dictionary<string, CoordinateContainerBase> currentGameBoardState)
        {
            base.BuildHits(currentGameBoardState);
            var markCallback = new Action<MarkCoordinateCallback>(base.WasHit);

            if (base.IsInHuntMode)
            {
                var hunterPrediction = base.PredictHunter(currentGameBoardState);
                return (hunterPrediction.Column, hunterPrediction.Row, markCallback);
            }

            var prediction = base.PredictBySimulation(currentGameBoardState);
            return (prediction.Column, prediction.Row, markCallback);
        }
    }
}