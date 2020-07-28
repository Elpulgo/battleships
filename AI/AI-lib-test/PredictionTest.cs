using Xunit;
using Core.Models;

namespace AI_lib_test
{
    public class PredictionTest : GameRunnerHelper
    {
        public GameBoardBase GameBoard { get; set; }
        public PredictionTest()
        {
            GameBoard = CreateGameBoard();
        }

        [Fact]
        public void Should_FinishRandomGameWithin_MaxBoxCount()
        {
            var numberOfMoves = base.RunRandomGame();
            Assert.True(numberOfMoves > 0);
        }

        [Fact]
        public void Should_FinishHunterGameWithin_MaxBoxCount()
        {
            var numberOfMoves = base.RunHunterGame();
            Assert.True(numberOfMoves > 0);
        }
    }
}
